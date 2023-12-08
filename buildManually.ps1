$godotProcess = Start-Process -FilePath $Env:GODOT -ArgumentList "--headless", "--export-release", "`"Windows Desktop`"" -PassThru
$godotProcess.WaitForExit()
