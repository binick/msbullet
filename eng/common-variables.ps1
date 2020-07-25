[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $RepoRoot,
    [Parameter(Mandatory = $true)]
    [string] $SourceBranch
)

$versionPath = Join-Path $RepoRoot -ChildPath version.json
    
$version = Get-Content -Raw -Path $versionPath | ConvertFrom-Json
    
$IsPublicRelease = ($version.publicReleaseRefSpec | ForEach-Object { $SourceBranch -match $_ }) -contains $true
    
Write-Host "##vso[task.setvariable variable=_PublicRelease;isOutput=true]$IsPublicRelease"
