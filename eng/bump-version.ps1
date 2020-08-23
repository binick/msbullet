[CmdletBinding(PositionalBinding = $false)]
Param(
    [string][Alias('v')] $version
)
    
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
        
    $VersionJson.version = "$($semVer.Major).$($semVer.Minor).$($semVer.Patch)"

    return $VersionJson
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

Set-Version $version | ConvertTo-Json | Set-Content -Path $VersionJsonPath

ExitWithExitCode 0
