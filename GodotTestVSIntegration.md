In order to integrate GodotTest from Chickensoft Jonna Banana May properly into Visual Studio, you may go through the installation instructions and then:

Click on "Extras" (Tools")

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/d8bb5b45-bc17-42b4-8595-37ad74980875)

then click on External Tools

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/299c8934-0c4a-4a21-90af-9ab29a0c2761)

Then we need a tiny translator-script so we can run single test-files and get them to work.
Save this script in "c:\dev\tools\RunGodotTests.ps1"
Ensure that the GODOT environment variable is set
```powershell
if (-not [System.Environment]::GetEnvironmentVariable('GODOT')) {
    Write-Host "Please ensure you have defined a GODOT system variable that leads to the path of the Godot executable file."
    exit
}

$godotPath = [System.Environment]::GetEnvironmentVariable('GODOT')

# Check if a file path was passed as an argument
if ($args.Length -eq 0) {
    Write-Host "No file specified. Running tests without specific file."
    Write-Host "Current Path: $(Get-Location)"
    & "$godotPath" --path . --run-tests --quit-on-finish
} else {
    $fullpath = $args[0]
    $filename = [System.IO.Path]::GetFileNameWithoutExtension($fullpath)
    Write-Host "Running Test: $filename"
    & "$godotPath" --path . --run-tests="$filename" --quit-on-finish
}
```

* Titel: 🔬 GodotTests - CurrentFile
* Command (Befehl): C:\windows\system32\windowspowershell\v1.0\powershell.exe
* Argumente (Arguments): -File "C:\dev\tools\RunGodotTests.ps1" $(ItemPath)
* Working Dir (Ausgangsverzeichnis): $(ProjectDir)

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/2afef892-340f-4c44-bafe-a200466208ff)

(Do the same thing for the GodotTests-All
* Titel:  🧪 GodotTests - All
* Command (Befehl): C:\windows\system32\windowspowershell\v1.0\powershell.exe
* Argumente (Arguments): -File "C:\dev\tools\RunGodotTests.ps1"
* Working Dir (Ausgangsverzeichnis): $(ProjectDir)

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/b6a9c80c-a0f9-4003-9a23-5554a184f90f)


Note the index Position in the external tools
Create Guid is number 1, so we are at number 4 and 5

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/057ad428-67bf-422b-8e55-00ebe4514cd7)

* now click on Tools (Extras in German) -> Customize (Anpassen) -> Commands (BEFEHLE)
* Klick on "KontextMenü" (Context Menu)
* Select the "Kontextmenüs für Projekte und Projektmappen | Element" or "ContextMenu for Projects and Project..Folders | Item"
* Then you can Scroll down a bit in the preview and select the "Start Tests" or "Tests Ausführen" so that we get sorted in that area
* then click on "Befehl hinzufügen" or "Add Command"


![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/c0fa1bbb-4fcb-4eeb-afd6-5d84b0119953)

Scroll a bit down to "Tools" (Extras in german) and select External Command (Number that you have noted before) here: Externer Befehl 4
Repeat that step for the External Command 5 (Externer Befehl 5) Or whatever your second number is.

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/8be33bb5-b692-4f1a-a312-f527936ff335)


Now it should appear in the context Menu of the right code side:

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/fa1c7415-c709-4216-9954-dc439036e8a8)
