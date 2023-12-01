# 🚀 Integrating GodotTest with Visual Studio

## Step-by-Step Guide

### 1. **Accessing External Tools**
- Navigate to `Extras` or in english: `Tools`.
- Click on `External Tools`.

![External Tools](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/299c8934-0c4a-4a21-90af-9ab29a0c2761)

### 2. **Setting Up the Translator Script**
- Save the following script as `c:\dev\tools\RunGodotTests.ps1`.
- Ensure the `GODOT` environment variable is set.

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
### 3. **Configuring GodotTests for Individual Files**
- **Title**: 🔬 GodotTests - CurrentFile
- **Command**: `C:\windows\system32\windowspowershell\v1.0\powershell.exe`
- **Arguments**: `-File "C:\dev\tools\RunGodotTests.ps1" $(ItemPath)`
- **Working Directory**: `$(ProjectDir)`

  ![Configuration Screenshot](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/2afef892-340f-4c44-bafe-a200466208ff)

### 4. **Configuring GodotTests for All Files**
- **Title**: 🧪 GodotTests - All
- **Command**: `C:\windows\system32\windowspowershell\v1.0\powershell.exe`
- **Arguments**: `-File "C:\dev\tools\RunGodotTests.ps1"`
- **Working Directory**: `$(ProjectDir)`

  ![Configuration Screenshot](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/b6a9c80c-a0f9-4003-9a23-5554a184f90f)

### 5. **Noting the Index Position in External Tools**
- Example: If Create Guid is number 1, then we are at number 4 and 5.

  ![Index Position Screenshot](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/057ad428-67bf-422b-8e55-00ebe4514cd7)
### 6. **Adding Commands to the Context Menu**
- Go to `Tools` > `Customize` > `Commands`.
- Select `KontextMenü` (Context Menu).
- Choose `ContextMenu for Projects and Project Folders | Item`.
- Scroll to `Start Tests` or `Tests Ausführen` and click on `Add Command`.

  ![Add Command Screenshot](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/c0fa1bbb-4fcb-4eeb-afd6-5d84b0119953)

### 7. **Finalizing the Context Menu**
- Scroll down to `Tools` and select `External Command 4`.
- Repeat for `External Command 5`.

  ![Finalize Context Menu](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/8be33bb5-b692-4f1a-a312-f527936ff335)

Now, the commands should appear in the context menu on the right side of the code.

![Context Menu Screenshot](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/fa1c7415-c709-4216-9954-dc439036e8a8)
