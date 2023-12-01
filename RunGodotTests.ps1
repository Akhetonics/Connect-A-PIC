# PowerShell-Skript: run-godot-tests.ps1

# Stelle sicher, dass die Umgebungsvariable GODOT gesetzt ist
if (-not [System.Environment]::GetEnvironmentVariable('GODOT')) {
    Write-Host "Please ensure you have defined a GODOT system variable that leads to the path of the Godot executable file."
    exit
}

$godotPath = [System.Environment]::GetEnvironmentVariable('GODOT')

# Überprüfe, ob ein Dateipfad als Argument übergeben wurde
if ($args.Length -eq 0) {
    Write-Host "No file specified. Running tests without specific file."
    & "$godotPath" --path . --run-tests --quit-on-finish
} else {
    $fullpath = $args[0]
    $filename = [System.IO.Path]::GetFileNameWithoutExtension($fullpath)
    Write-Host "Running Test: $filename"
    & "$godotPath" --path . --run-tests="$filename" --quit-on-finish
}