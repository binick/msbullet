[CmdletBinding(PositionalBinding = $false)]
Param(
    [string][Alias('v')] $version,
    [switch][Alias('f')] $force = $false
)
    
if ($version -notmatch '^\d+.\d+.\d+') {
    Write-Host "Version must respect this sintax '{major}.{minor}' or '{major}.{minor}.{patch}'"
    ExitWithExitCode 1
}

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$VersionJsonPath = Join-Path $RepoRoot 'version.json'

function Set-Version($version) {
    if (!$version) {
        Write-Host "Need to specify semantic version to bump version."
        ExitWithExitCode 1
    }
    
    try {
        $semVer = ConvertTo-SemanticVersion $version
    }
    catch {
        Write-Host "Version $($version) isn't a valid semantic version."
        ExitWithExitCode 1
    }
        
    if ($semVer.CompareTo($(Get-CurrentVersion)) -le 0) {
        Write-Host "Bump version must be greater then $($VersionJson.version)"
        ExitWithExitCode 1
    }
        
    Set-RawVersion "$($semVer.Major).$($semVer.Minor).$($semVer.Patch)"
}

function Set-RawVersion($version) {
    $VersionJson.version = $version
}

function Get-CurrentVersion() {
    return ConvertTo-SemanticVersion $(Normilize-NerdBankVersion $VersionJson.version)
}

function ConvertTo-SemanticVersion($version) {
    return [System.Management.Automation.SemanticVersion]::new($version)
}

function Normilize-NerdBankVersion ($rawVersion) {
    return $rawVersion -replace '.{height}', ''
}

$VersionJson = Get-Content -Raw -Path $VersionJsonPath | ConvertFrom-Json

if (!$force) {
    Set-Version $version
}
else {
    Set-RawVersion $version
}

$VersionJson | ConvertTo-Json | Set-Content -Path $VersionJsonPath
