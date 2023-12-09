# Connect-A-PIC
![image](badges/line_coverage.svg)
![image](badges/branch_coverage.svg)

Gamification of PIC Design (PIC = Photonic integrated circuit)

## Current Version

Version: <!--version-->0.0.28<!--version-->

![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/cde9edb5-f93c-452d-b046-a02134fbc1ba)

## Downloads

You can download the newest releases [here](https://github.com/Akhetonics/Connect-A-PIC/releases).

# About Connect a PIC

This Software wants to provide a very simple way of designing optical circuits on a chip that is then printable using our export to Nazca-Python function
The designer is written in C# Godot 4 and consists of three projects: 

* The "CAP-Core" project (the model) has all the knowledge of where in the field components are placed,		 
	* it takes care of the Grid [i,j] and it has a function to calculate a Scattered Matrix. 
	* This S-Matrix can calculate the light-distribution of all light that goes through all the components. 
	* Each component has a smaller S-Matrix that tells the engine how exactly the light goes from each pin of the component to each other pin - like if the phase shifts or if the light's intensity gets less through the path from one pin to the other pin inside of the component. 
    * The Model also has functions to export the whole game-grid to a python script that then can be used to actually manufacture the chip by sending it to a manufacturer. 
* Then there is the view and the integrationtests of the Model - the view has no unit tests as I didn't really find a testing suite for godot 4 - this was also one reason to separate the core functionality from the godot 4 engine. 
* There also is a shader for mixing the light colors properly together that uses phase calculations based on blue and red spheres

# Target Audience
Our Target Audience is students and scientists that are interested in photonics.

# Setup Code
* Install Visual Studio 
* Install Godot Engine and add a System Variable called "GODOT" to the Path to the godot mono exe file like so:
	* open your command prompt (WINDOWS+R, type 'cmd' and press enter)
```shell
setx GODOT "C:\Program Files\Godot_v4.1\Godot_v4.1.exe" /M
```
* Clone this project into a nice folder
* Setup Launch Profile for visual Studio where 
	* "Path to the executable to run" is the Path to your godot_v4.0-beta_mono_win64.exe 
	* Command Line Arguments is `--path . --verbose`
	* Working Directory is '.'
* Install Submodules using 'git submodule init' and then 'git submodule update --recursive --remote'
 * [Create two external Tools in Visual studio](https://github.com/Akhetonics/Connect-A-PIC/blob/feature/ComponentPackaging/GodotTestVSIntegration.md) to be able to manually run the GoDotTest - Unit tests for Godot.
* Install Submodules using `git submodule init` and then `git submodule update --recursive --remote`
* Coverlet is a tool for code-coverage reports - install it like using powershell like this:
```shell
dotnet tool install --global coverlet.console
dotnet tool update --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet tool update --global dotnet-reportgenerator-globaltool
```

# Install Nazca 
In order to be able to compile the exported Nazca code, you want to have Nazca installed properly
https://nazca-design.org/installation/

# Tools we use
In this project we are using GodotTestDriver, GoDotTest, Shouldly, maybe Coverlet
Chickensoft has some approach for unit testing: https://github.com/chickensoft-games/GoDotTest
