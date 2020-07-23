[CmdletBinding(PositionalBinding = $false)]
Param(
    [string][Alias('c')] $configuration = "Debug",
    [string] $platform = $null,
    [string][Alias('v')] $verbosity = "minimal",
    [bool] $warnAsError = $true,
    [bool] $nodeReuse = $true,
    [switch][Alias('r')] $restore,
    [switch][Alias('b')] $build,
    [switch] $rebuild,
    [switch][Alias('t')] $test,
    [switch] $integrationTest,
    [switch] $collect,
    [switch] $pack,
    [switch] $clean,
    [switch][Alias('bl')] $binaryLog,
    [switch] $ci,
    [switch] $prepareMachine,
    [switch][Alias('h')] $help,
    [Parameter(ValueFromRemainingArguments = $true)][String[]] $properties
)

function Print-Usage() {
    Write-Host "Common settings:"
    Write-Host "  -configuration <value>  Build configuration: 'Debug' or 'Release' (short: -c)"
    Write-Host "  -platform <value>       Platform configuration: 'x86', 'x64' or any valid Platform value to pass to msbuild"
    Write-Host "  -verbosity <value>      Msbuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
    Write-Host "  -binaryLog              Output binary log (short: -bl)"
    Write-Host "  -help                   Print help and exit"
    Write-Host ""

    Write-Host "Actions:"
    Write-Host "  -restore                Restore dependencies (short: -r)"
    Write-Host "  -build                  Build solution (short: -b)"
    Write-Host "  -rebuild                Rebuild solution"
    Write-Host "  -test                   Run all unit tests in the solution (short: -t)"
    Write-Host "  -integrationTest        Run all integration tests in the solution"
    Write-Host "  -performanceTest        Run all performance tests in the solution [WIP]"
    Write-Host "  -collect                Collect code coverage metrics for all unit tests in the solution"
    Write-Host "  -pack                   Package build outputs into NuGet packages"
    Write-Host "  -sign                   Sign build outputs [WIP]"
    Write-Host "  -clean                  Clean the solution"
    Write-Host ""

    Write-Host "Advanced settings:"
    Write-Host "  -ci                     Set when running on CI server"
    Write-Host "  -prepareMachine         Prepare machine for CI run, clean up processes after build"
    Write-Host "  -warnAsError <value>    Sets warnaserror msbuild parameter ('true' or 'false')"
    Write-Host ""

    Write-Host "Command line arguments not listed above are passed thru to msbuild."
    Write-Host "The above arguments can be shortened as much as to be unambiguous (e.g. -co for configuration, -t for test, etc.)."
}

. $PSScriptRoot\tools.ps1

function Build {
    $bl = if ($binaryLog) { '/bl:' + (Join-Path $LogDir 'Build.binlog') } else { '' }
    
    $parametersArgs = @()
    if ($platform) { $parametersArgs += "/p:Platform=$platform" }
    if ($configuration) { $parametersArgs += "/p:Configuration=$configuration" }

    $targets = @()
    if ($restore) { $targets += 'Restore' }
    if ($build) { $targets += 'Build' }
    if ($rebuild) { $targets += 'Rebuild' }
    if ($test) { $targets += 'Test' }
    if ($integrationTest) { $targets += 'IntegrationTest' }
    if ($collect) { $targets += 'CollectCoverage' }
    if ($pack) { $targets += 'Pack' }
    
    $targetsArgs = @()
    if ($targets.Count -ne 0) {  
        $targetsArgs += $targets.ForEach( { "/t:$_" } )
    }

    MSBuild `
        $bl `
        @parametersArgs `
        @targetsArgs `
        @properties
}

try {
    if ($clean) {
        if (Test-Path $ArtifactsDir) {
            Remove-Item -Recurse -Force $ArtifactsDir
            Write-Host 'Artifacts directory deleted.'
        }
        exit 0
    }

    if ($help -or (($null -ne $properties) -and ($properties.Contains('/help') -or $properties.Contains('/?')))) {
        Print-Usage
        exit 0
    }

    if ($ci) {
        $binaryLog = $true
        $nodeReuse = $false
    }
    
    Build
}
catch {
    Write-Host $_.ScriptStackTrace
    Write-Error -Message $_
    ExitWithExitCode 1
}

ExitWithExitCode 0
