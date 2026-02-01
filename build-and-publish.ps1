param (
    [string]$ProjectPath = "Software/PanelSMS.csproj",
    [string]$Runtime = "linux-x64",
    [string]$PublishDirName = "_docker_publish\_publish"
)

$ErrorActionPreference = "Stop"

Clear-Host

$banner = @'
         
                           powered by MohammadReveshtian.ir
'@

Write-Host $banner -ForegroundColor Cyan
Write-Host "`n                         reveshtiandev`n" -ForegroundColor Yellow

$semverPattern = '^\d+\.\d+\.\d+(-[0-9A-Za-z\.-]+)?$'

do {
    $version = Read-Host "Enter version (e.g. 1.0.0 or 1.0.0-beta)"
    if ($version -match $semverPattern) {
        break
    }
    Write-Host "Invalid version format. Use something like 1.0.0 or 1.0.0-beta." -ForegroundColor Red
} while ($true)

Write-Host "Using version: $version" -ForegroundColor Green

$env:VERSION = $version

$repoRoot = $PSScriptRoot
$projectFullPath = Join-Path $repoRoot $ProjectPath
$publishDir = Join-Path $repoRoot $PublishDirName  

Write-Host "Project: $projectFullPath"
Write-Host "Publish directory: $publishDir"

if (Test-Path $publishDir) {
    Write-Host "Cleaning publish directory: $publishDir" -ForegroundColor Yellow
    Get-ChildItem $publishDir -Recurse -Force | Remove-Item -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
}

Write-Host "Running dotnet publish..." -ForegroundColor Cyan

dotnet publish $projectFullPath `
    -c Release `
    -r $Runtime `
    -v minimal `
    -p:UseAppHost=false `
    -p:DebugType=none `
    /p:Version="$version" `
    /p:InformationalVersion="$version" `
    --output "$publishDir"

if ($LASTEXITCODE -ne 0) {
    Write-Host "dotnet publish failed with exit code $LASTEXITCODE. Aborting pipeline." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "dotnet publish completed successfully." -ForegroundColor Green

$imageName = "samp.core.backend"  

Write-Host "Building docker image ${imageName}:$version and ${imageName}:latest ..." -ForegroundColor Cyan

docker build `
    --platform=linux/amd64 `
    --build-arg APP_VERSION="$version" `
    --build-arg BUILD_DATE="$(Get-Date -Format o)" `
    -t "${imageName}:$version" `
    -t "${imageName}:latest" `
    -f (Join-Path $repoRoot "HostRuntime.Dockerfile") `
    "$publishDir"

if ($LASTEXITCODE -ne 0) {
    Write-Host "docker build failed with exit code $LASTEXITCODE. Aborting pipeline." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Docker build completed successfully." -ForegroundColor Green

$dockerPublishRoot = Join-Path $repoRoot "_docker_publish"
$versionDir = Join-Path $dockerPublishRoot $version

if (-not (Test-Path $versionDir)) {
    New-Item -ItemType Directory -Path $versionDir -Force | Out-Null
}

$tarVersionPath = Join-Path $versionDir "PanelSMS-$version.tar"

Write-Host "Saving docker image ${imageName}:$version to $tarVersionPath" -ForegroundColor Cyan
docker save "${imageName}:$version" -o "$tarVersionPath"
if ($LASTEXITCODE -ne 0) {
    Write-Host "docker save for ${imageName}:$version failed with exit code $LASTEXITCODE. Aborting pipeline." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "All done. Tar files are in: $versionDir" -ForegroundColor Green
