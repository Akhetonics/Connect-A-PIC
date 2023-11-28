@echo off
echo please ensure you have defined a GODOT system variable defined that leads to the path of the godot exe file
"%GODOT%" --path . --run-tests --quit-on-finish
pause