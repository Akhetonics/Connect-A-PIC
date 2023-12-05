# PowerShell-Skript: run-godot-tests.ps1

# make sure that environment variable GODOT is set
if (-not [System.Environment]::GetEnvironmentVariable('GODOT')) {
    Write-Host "Please ensure you have defined a GODOT system variable that leads to the path of the Godot executable file."
    exit
}

$godotPath = [System.Environment]::GetEnvironmentVariable('GODOT')

# test if a file path was given as a parameter
if ($args.Length -eq 0) {
    Write-Host "No file specified. Running tests without specific file."
    & "$godotPath" --path . --run-tests --quit-on-finish
} else {
    $fullpath = $args[0]
    $filename = [System.IO.Path]::GetFileNameWithoutExtension($fullpath)
    Write-Host "Running Test: $filename"
    & "$godotPath" --path . --run-tests="$filename" --quit-on-finish
}