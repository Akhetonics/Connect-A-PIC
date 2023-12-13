# Adjust the Godot path and arguments as per your local setup
$godotPath = $env:GODOT
$releaseDir = "Release"

# Extract Version from Project File
$version = Select-String -Path "Connect-A-PIC.csproj" -Pattern '<Version>(.*)</Version>' | % { $_.Matches.Groups[1].Value }
Write-Host "The extracted project version is $version"

# Read Godot version from global.json
$godotVersion = Get-Content -Raw -Path 'global.json' | ConvertFrom-Json | Select-Object -ExpandProperty 'msbuild-sdks' | Select-Object -ExpandProperty 'Godot.NET.Sdk'
Write-Host "Godot version: $godotVersion"

# Delete all old Releases
if (Test-Path $releaseDir) {
    Remove-Item -Path "Release\*" -Recurse -Force
    Write-Host "Release Folder got cleared"
}

# Create Build Directory
if (-not (Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir
}

# Build Windows EXE
$godotArgs = "--headless", "--export-release", "`"Windows Desktop`"", "--path .", "--verbose"
$godotProcess = Start-Process -FilePath $godotPath -ArgumentList $godotArgs -PassThru
$godotProcess.WaitForExit()

# Zip Release Build
$zipPath = Join-Path "$releaseDir" "ConnectAPIC.zip"
Compress-Archive -Path "$releaseDir\*" -DestinationPath $zipPath

# List files in Release Directory
Write-Host "list files in folder $releaseDir"
Get-ChildItem -Path $releaseDir -Recurse | ForEach-Object { Write-Host $_.FullName }


Write-Host "Press any key to continue..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")