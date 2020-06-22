# Initialize variables if they aren't already defined.
# These may be defined as parameters of the importing script, or set after importing this script.

# CI mode - set to true on CI server for PR validation build or official build.
[bool]$ci = if (Test-Path variable:ci) { $ci } else { $false }

# Build configuration. Common values include 'Debug' and 'Release', but the repository may use other names.
[string]$configuration = if (Test-Path variable:configuration) { $configuration } else { 'Debug' }

# Set to true to output binary log from msbuild. Note that emitting binary log slows down the build.
# Binary log must be enabled on CI.
[bool]$binaryLog = if (Test-Path variable:binaryLog) { $binaryLog } else { $ci }

# True to restore toolsets and dependencies.
[bool]$restore = if (Test-Path variable:restore) { $restore } else { $true }

# Adjusts msbuild verbosity level.
[string]$verbosity = if (Test-Path variable:verbosity) { $verbosity } else { 'minimal' }

# Configures warning treatment in msbuild.
[bool]$warnAsError = if (Test-Path variable:warnAsError) { $warnAsError } else { $true }

# True to attempt using .NET Core already that meets requirements specified in global.json
# installed on the machine instead of downloading one.
[bool]$useInstalledDotNetCli = if (Test-Path variable:useInstalledDotNetCli) { $useInstalledDotNetCli } else { $true }

# Enable repos to use a particular version of the on-line dotnet-install scripts.
#    default URL: https://dot.net/v1/dotnet-install.{ps1|sh} OS dependent
[string]$dotnetInstallScriptVersion = if (Test-Path variable:dotnetInstallScriptVersion) { $dotnetInstallScriptVersion } else { 'v1' }

# True to use global NuGet cache instead of restoring packages to repository-local directory.
[bool]$useGlobalNuGetCache = if (Test-Path variable:useGlobalNuGetCache) { $useGlobalNuGetCache } else { !$ci }

# An array of names of processes to stop on script exit if prepareMachine is true.
$processesToStopOnExit = if (Test-Path variable:processesToStopOnExit) { $processesToStopOnExit } else { @('msbuild', 'dotnet') }

set-strictmode -version 2.0
$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Create-Directory([string[]] $path) {
  if (!(Test-Path $path)) {
    New-Item -path $path -force -itemType 'Directory' | Out-Null
  }
}

# createSdkLocationFile parameter enables a file being generated under the toolset directory
# which writes the sdk's location into. This is only necessary for cmd --> powershell invocations
# as dot sourcing isn't possible.
function InitializeDotNetCli([bool]$install, [bool]$createSdkLocationFile) {
  if (Test-Path variable:global:_DotNetInstallDir) {
    return $global:_DotNetInstallDir
  }

  # Don't resolve runtime, shared framework, or SDK from other locations to ensure build determinism
  $env:DOTNET_MULTILEVEL_LOOKUP = 0

  # Disable first run since we do not need all ASP.NET packages restored.
  $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1

  # Disable telemetry on CI.
  if ($ci) {
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
  }

  # Source Build uses DotNetCoreSdkDir variable
  if ($null -ne $env:DotNetCoreSdkDir) {
    $env:DOTNET_INSTALL_DIR = $env:DotNetCoreSdkDir
  }

  # Find the first path on %PATH% that contains the dotnet.exe
  if ($useInstalledDotNetCli -and (-not $globalJsonHasRuntimes) -and ($null -eq $env:DOTNET_INSTALL_DIR)) {
    $dotnetCmd = Get-Command 'dotnet.exe' -ErrorAction SilentlyContinue
    if ($null -ne $dotnetCmd) {
      $env:DOTNET_INSTALL_DIR = Split-Path $dotnetCmd.Path -Parent
    }
  }

  $dotnetSdkVersion = $GlobalJson.tools.dotnet

  # Use dotnet installation specified in DOTNET_INSTALL_DIR if it contains the required SDK version,
  # otherwise install the dotnet CLI and SDK to repo local .dotnet directory to avoid potential permission issues.
  if ((-not $globalJsonHasRuntimes) -and ($null -ne $env:DOTNET_INSTALL_DIR) -and (Test-Path(Join-Path $env:DOTNET_INSTALL_DIR "sdk\$dotnetSdkVersion"))) {
    $dotnetRoot = $env:DOTNET_INSTALL_DIR
  }
  else {
    $dotnetRoot = Join-Path $RepoRoot '.dotnet'

    if (-not (Test-Path(Join-Path $dotnetRoot "sdk\$dotnetSdkVersion"))) {
      if ($install) {
        InstallDotNetSdk $dotnetRoot $dotnetSdkVersion
      }
      else {
        Write-Host -Message "Unable to find dotnet with SDK version '$dotnetSdkVersion'"
        ExitWithExitCode 1
      }
    }

    $env:DOTNET_INSTALL_DIR = $dotnetRoot
  }

  # Creates a temporary file under the toolset dir.
  # The following code block is protecting against concurrent access so that this function can
  # be called in parallel.
  if ($createSdkLocationFile) {
    do {
      $sdkCacheFileTemp = Join-Path $ToolsetDir $([System.IO.Path]::GetRandomFileName())
    }
    until (!(Test-Path $sdkCacheFileTemp))
    Set-Content -Path $sdkCacheFileTemp -Value $dotnetRoot

    try {
      Rename-Item -Force -Path $sdkCacheFileTemp 'sdk.txt'
    }
    catch {
      # Somebody beat us
      Remove-Item -Path $sdkCacheFileTemp
    }
  }

  # Add dotnet to PATH. This prevents any bare invocation of dotnet in custom
  # build steps from using anything other than what we've downloaded.
  # It also ensures that VS msbuild will use the downloaded sdk targets.
  $env:PATH = "$dotnetRoot;$env:PATH"

  return $global:_DotNetInstallDir = $dotnetRoot
}

function GetDotNetInstallScript([string] $dotnetRoot) {
  $installerOsExt = if ($env:OS -eq 'Windows_NT') { 'ps1' } else { 'sh' }

  $installScript = Join-Path $dotnetRoot "dotnet-install.$installerOsExt"
  if (!(Test-Path $installScript)) {
    Create-Directory $dotnetRoot
    $ProgressPreference = 'SilentlyContinue' # Don't display the console progress UI - it's a huge perf hit

    $maxRetries = 5
    $retries = 1

    $uri = "https://dot.net/$dotnetInstallScriptVersion/dotnet-install.$installerOsExt"

    while ($true) {
      try {
        Write-Host "GET $uri"
        Invoke-WebRequest $uri -OutFile $installScript
        break
      }
      catch {
        Write-Host "Failed to download '$uri'"
        Write-Error $_.Exception.Message -ErrorAction Continue
      }

      if (++$retries -le $maxRetries) {
        $delayInSeconds = [math]::Pow(2, $retries) - 1 # Exponential backoff
        Write-Host "Retrying. Waiting for $delayInSeconds seconds before next attempt ($retries of $maxRetries)."
        Start-Sleep -Seconds $delayInSeconds
      }
      else {
        throw "Unable to download file in $maxRetries attempts."
      }

    }
  }

  return $installScript
}

function InstallDotNetSdk([string] $dotnetRoot, [string] $version, [string] $architecture = '') {
  InstallDotNet $dotnetRoot $version $architecture
}

function InstallDotNet([string] $dotnetRoot,
  [string] $version,
  [string] $architecture = '',
  [string] $runtime = '',
  [bool] $skipNonVersionedFiles = $false,
  [string] $runtimeSourceFeed = '',
  [string] $runtimeSourceFeedKey = '') {

  $installScript = GetDotNetInstallScript $dotnetRoot
  $installParameters = @{
    Version    = $version
    InstallDir = $dotnetRoot
  }

  if ($architecture) { $installParameters.Architecture = $architecture }
  if ($runtime) { $installParameters.Runtime = $runtime }
  if ($skipNonVersionedFiles) { $installParameters.SkipNonVersionedFiles = $skipNonVersionedFiles }

  try {
    & $installScript @installParameters
  }
  catch {
    Write-Host -Message "Failed to install dotnet runtime '$runtime' from public location."

    # Only the runtime can be installed from a custom [private] location.
    if ($runtime -and ($runtimeSourceFeed -or $runtimeSourceFeedKey)) {
      if ($runtimeSourceFeed) { $installParameters.AzureFeed = $runtimeSourceFeed }

      if ($runtimeSourceFeedKey) {
        $decodedBytes = [System.Convert]::FromBase64String($runtimeSourceFeedKey)
        $decodedString = [System.Text.Encoding]::UTF8.GetString($decodedBytes)
        $installParameters.FeedCredential = $decodedString
      }

      try {
        & $installScript @installParameters
      }
      catch {
        Write-Host -Message "Failed to install dotnet runtime '$runtime' from custom location '$runtimeSourceFeed'."
        ExitWithExitCode 1
      }
    }
    else {
      ExitWithExitCode 1
    }
  }
}

function InitializeBuildTool() {
  if (Test-Path variable:global:_BuildTool) {
    return $global:_BuildTool
  }

  # Initialize dotnet cli if listed in 'tools'
  $dotnetRoot = $null
  if (Get-Member -InputObject $GlobalJson.tools -Name 'dotnet') {
    $dotnetRoot = InitializeDotNetCli -install:$restore
  }

  if (!$dotnetRoot) {
    Write-Host -Message "/global.json must specify 'tools.dotnet'."
    ExitWithExitCode 1
  }
  $buildTool = @{ Path = Join-Path $dotnetRoot 'dotnet'; Command = 'msbuild'; Tool = 'dotnet'; Framework = 'netcoreapp2.1' }

  return $global:_BuildTool = $buildTool
}

# This will exec a process using the console and return it's exit code.
# This will not throw when the process fails.
# Returns process exit code.
function Exec-Process([string]$command, [string]$commandArgs) {
  $startInfo = New-Object System.Diagnostics.ProcessStartInfo
  $startInfo.FileName = $command
  $startInfo.Arguments = $commandArgs
  $startInfo.UseShellExecute = $false
  $startInfo.WorkingDirectory = Get-Location

  $process = New-Object System.Diagnostics.Process
  $process.StartInfo = $startInfo
  $process.Start() | Out-Null

  $finished = $false
  try {
    while (-not $process.WaitForExit(100)) {
      # Non-blocking loop done to allow ctr-c interrupts
    }

    $finished = $true
    return $global:LASTEXITCODE = $process.ExitCode
  }
  finally {
    # If we didn't finish then an error occurred or the user hit ctrl-c.  Either
    # way kill the process
    if (-not $finished) {
      $process.Kill()
    }
  }
}

function ExitWithExitCode([int] $exitCode) {
  if ($ci -and $prepareMachine) {
    Stop-Processes
  }
  exit $exitCode
}

function Stop-Processes() {
  Write-Host 'Killing running build processes...'
  foreach ($processName in $processesToStopOnExit) {
    Get-Process -Name $processName -ErrorAction SilentlyContinue | Stop-Process
  }
}

#
# Executes msbuild (or 'dotnet msbuild') with arguments passed to the function.
# The arguments are automatically quoted.
# Terminates the script if the build fails.
#
function MSBuild() {
  if ($ci) {
    $buildTool = InitializeBuildTool

    # Work around issues with Azure Artifacts credential provider
    # https://github.com/dotnet/arcade/issues/3932
    if ($ci -and $buildTool.Tool -eq 'dotnet') {
      dotnet nuget locals http-cache -c

      $env:NUGET_PLUGIN_HANDSHAKE_TIMEOUT_IN_SECONDS = 20
      $env:NUGET_PLUGIN_REQUEST_TIMEOUT_IN_SECONDS = 20
    }
  }

  MSBuild-Core @args
}

#
# Executes msbuild (or 'dotnet msbuild') with arguments passed to the function.
# The arguments are automatically quoted.
# Terminates the script if the build fails.
#
function MSBuild-Core() {
  if ($ci) {
    if (!$binaryLog) {
      Write-Host -Message 'Binary log must be enabled in CI build.'
      ExitWithExitCode 1
    }

    if ($nodeReuse) {
      Write-Host -Message 'Node reuse must be disabled in CI build.'
      ExitWithExitCode 1
    }
  }

  $buildTool = InitializeBuildTool

  $cmdArgs = "$($buildTool.Command) /m /nologo /clp:Summary /v:$verbosity /nr:$nodeReuse /p:ContinuousIntegrationBuild=$ci"

  if ($warnAsError) {
    $cmdArgs += ' /warnaserror /p:TreatWarningsAsErrors=true'
  }
  else {
    $cmdArgs += ' /p:TreatWarningsAsErrors=false'
  }

  foreach ($arg in $args) {
    if ($null -ne $arg -and $arg.Trim() -ne "") {
      $cmdArgs += " `"$arg`""
    }
  }

  $exitCode = Exec-Process $buildTool.Path $cmdArgs

  if ($exitCode -ne 0) {
    Write-Host -Message 'Build failed.'

    $buildLog = GetMSBuildBinaryLogCommandLineArgument $args
    if ($null -ne $buildLog) {
      Write-Host "See log: $buildLog" -ForegroundColor DarkGray
    }

    ExitWithExitCode $exitCode
  }
}

function GetMSBuildBinaryLogCommandLineArgument($arguments) {
  foreach ($argument in $arguments) {
    if ($null -ne $argument) {
      $arg = $argument.Trim()
      if ($arg.StartsWith('/bl:', "OrdinalIgnoreCase")) {
        return $arg.Substring('/bl:'.Length)
      }

      if ($arg.StartsWith('/binaryLogger:', 'OrdinalIgnoreCase')) {
        return $arg.Substring('/binaryLogger:'.Length)
      }
    }
  }

  return $null
}

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$EngRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$ArtifactsDir = Join-Path $RepoRoot 'artifacts'
$ToolsetDir = Join-Path $ArtifactsDir 'toolset'
$ToolsDir = Join-Path $RepoRoot '.tools'
$LogDir = Join-Path (Join-Path $ArtifactsDir 'log') $configuration
$TempDir = Join-Path (Join-Path $ArtifactsDir 'tmp') $configuration
$GlobalJson = Get-Content -Raw -Path (Join-Path $RepoRoot 'global.json') | ConvertFrom-Json
# true if global.json contains a "runtimes" section
$globalJsonHasRuntimes = if ($GlobalJson.tools.PSObject.Properties.Name -Match 'runtimes') { $true } else { $false }

Create-Directory $ToolsetDir
Create-Directory $TempDir
Create-Directory $LogDir
