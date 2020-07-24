[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $RepoRoot
)

$versionPath = Join-Path $RepoRoot -ChildPath version.json
    
$version = Get-Content -Raw -Path $versionPath | ConvertFrom-Json
    
$IsPublicRelease = ($version.publicReleaseRefSpec | ForEach-Object { $branch -match $_ }) -contains $true
    
Write-Host "##vso[task.setvariable variable=_PublicRelease;isOutput=true]$IsPublicRelease"
