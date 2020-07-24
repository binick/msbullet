$version = Get-Content -Raw -Path ../version.json | ConvertFrom-Json

$IsPublicRelease = ($version.publicReleaseRefSpec | ForEach-Object { $branch -match $_ }) -contains $true

Write-Host "##vso[task.setvariable variable=_PublicRelease]$IsPublicRelease"
