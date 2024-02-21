# Connect-A-PIC
![image](badges/line_coverage.svg)
![image](badges/branch_coverage.svg)

Gamification of PIC Design (PIC = Photonic integrated circuit)

## Current Version

Version: <!--version-->0.0.54<!--version-->

[![image](https://github.com/Akhetonics/Connect-A-PIC/assets/18228325/2615d391-ab97-4c42-9234-11cc0b130b2c)](https://github.com/Akhetonics/Connect-A-PIC/releases)


## Downloads

You can download the newest releases [here](https://github.com/Akhetonics/Connect-A-PIC/releases).

# About Connect a PIC

This software aims to simplify the design of optical circuits on a chip, which can then be printed using our Nazca-Python export function.
The designer, developed in C# using Godot 4, consists of three distinct projects

* The "CAP-Core" project (the model) has all the knowledge of where in the field components are placed,         
    * it takes care of the Grid [i,j] and it has a function to calculate a Scattered Matrix. 
    * This S-Matrix can calculate the light-distribution of all light that goes through all the components. 
    * Each component has a smaller S-Matrix that tells the engine how exactly the light goes from each pin of the component to each other pin - like if the phase shifts or if the light's intensity gets less through the path from one pin to the other pin inside of the component. 
    * The Model also has functions to export the whole game-grid to a python script that then can be used to actually manufacture the chip by sending it to a manufacturer. 
* Then there is the view and the integrationtests of the Model - the view has no unit tests as I didn't really find a testing suite for godot 4 - this was also one reason to separate the core functionality from the godot 4 engine. 
* There also is a shader for mixing the light colors properly together that uses phase calculations based on blue and red spheres

# Target Audience
Our Target Audience is students and scientists that are interested in photonics.

# Setup Dependencies and Editor
* Install Visual Studio 
* Download the Visual Studio components: "Game Development with Unity" just in case
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
* git clone this project
Install submodules by executing 'git submodule init', followed by 'git submodule update --recursive --remote'.
 * [Create two external Tools in Visual studio](https://github.com/Akhetonics/Connect-A-PIC/blob/feature/ComponentPackaging/GodotTestVSIntegration.md) to be able to manually run the GoDotTest - Unit tests for Godot via a popup menu.
* Coverlet is a tool for code-coverage reports - install it like using powershell like this:
```shell
dotnet tool install --global coverlet.console
dotnet tool update --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet tool update --global dotnet-reportgenerator-globaltool
```

# Install Nazca 
Connect-A-PIC can export the whole circuit that one has built into python code that uses the photonic design library "Nazca".
In order to be able to compile the exported Nazca code, you want to have Nazca installed properly
https://nazca-design.org/installation/
you will also need to start visual studio code or your preferred editor through anaconda and then compile the exported python code - for now the components in the PDK are but examples as the exact PDK may depend on the fab you are using, but you can open the GDS files and edit the Nazca code or the GDS files as you wish.

# Tools we use
In this project we are using GodotTestDriver, GoDotTest, Shouldly, Coverlet, XUnit
Chickensoft has some approach for unit testing: https://github.com/chickensoft-games/GoDotTest

# Components
We are using component files that one can create using the GODOT Editor and then compile to a *.PCK file.
PCK files will be loaded into the Connect-A-PIC toolbox as soon as they are located in the Scenes/Components - folder of the project at compile time
Each PCK file consists of a folderstructure that has to match exactly the Godot folder structure of the main program which is "Scenes/Components/[ComponentName]/".
You can see example components in the other Github project ["CAP- Standard Components"](https://github.com/Akhetonics/Connect-A-PIC_StandardComponents)
## JSON file
There is a JSON file that describes the exact behaviour of the component and it resides within the Scenes/Components/[ComponentName] folder.
It does not matter what the name of this file is as long as there is no other JSON file with the *.JSON extension in that folder.
just have a look at one of those JSON files there. Keep in mind, that if you want to define a non-linear-connection between two pins in that JSON file, you can only do it only like so:
``` JSON
// ...
"sMatrices": [
    {
    "waveLength" : 1550,
    "connections": [
        {
        "fromPinNr": 0,
        "toPinNr": 1,
        "magnitude": 0.5,
        "phase": 1.2161003820350373,
        "nonLinearFormula" : "Div(1,Add(PIN1,PIN2))"
        },
    ]
    }
    ]
// ...
```
You don't have to use "magnitude" and "phase", but can use "real" and "imaginary" instead.
the nonLinearFormula only supports the functions Add, Sub, Div and Mul for now and those are case sensitive with the first letter being capital and the others are small. 

# github actions
there are several github actions for release, spell check, XUnit tests (for all non-Godot functions) and GoDotTest tests (for all Nodes and for integration tests).
## spellcheck
After every commit, github scans all files for spelling errors. If you want to add a word to the dictionary, you can add it to the "cspell.json" file or you can exclude whole extensions from being spell checked.

## release on Github
To release this software on github, you simply have to update the version using the github action "version change" to a higher number as before in the SemVer format.
It will then update the software version in the solution file and if all tests are passing it will be compiled, zipped and uploaded to the github's release section.

# SubProjects
This tool consists of five sub projects mainly:

## CAP_Contracts
Here you can add all interfaces that are needed in the other projects as well.
## CAP-Core
All business logic is being calculated here - this makes it easy to move the project to another frontend like Unity or even Blazor so that one can use Connect-A-PIC via the browser.
Mainly the light distribution is being calculated here, the Nazca export, and the grid management. Everything model/business related is here.
## CAP-DataAccess (Layer)
Godot for example uses their own file storage structure so we should not simply assume that the harddrive is our main target to store and read data using the File namespace directly. 
This project handles all data access, finds JSON and PCK files in (virtual) folders, it validates the JSON files and has Data Transfer Objects (DTOs) for all parts of the JSON structure that is used to load the components from disk. It can also convert a ComponentDraft (the Json - structure) into a real Component.
## Connect-A-PIC
The Entry point of this software is the Main.cs in the Main.tscn scene file that only destinguishes if you want to run the GodotTest unit tests or to run the PICEditor.tscn editor scene instead.
If you are starting the software without parameters it starts the PICEditor.tscn and initializes everything in the GameManager.cs class.
It will load all PCK files, import them into godot, then search for all JSON files in the GODOT-Filesystem, read them, load all Components into the ComponentViewFactory and into the ComponentFactory.
The Model can instantiate new components and the View listenes to event registered by the "ViewModel" that the model is firing up whenever there are new Components or when they rotate or when they get deleted.

## UnitTests
All tests for the Model or "CAP-Core" can be found here, simply use Visual Studio and start them. We use XUnit to test all business logic and [GoDotTest](https://github.com/chickensoft-games/GoDotTest) to test some features of the frontend, that "View" - mostly if the shaders are visible correctly.


# How to Contribute

We welcome contributions to this project. If you're interested in helping, you can [contribute in the following ways](https://github.com/Akhetonics/Connect-A-PIC/wiki/CONTRIBUTING):

1. **Pull Requests:** 
    - For any changes or improvements, please submit a pull request. 
    - Ensure that each pull request addresses a specific issue or feature. Avoid combining multiple changes into a single request to facilitate review and integration.
    - Please provide a clear and detailed description of your changes and the purpose behind them.

2. **Reporting Issues:**
    - If you find bugs or have suggestions for improvements, feel free to open an issue in the project's issue tracker.

3. **Code Guidelines:**
    - Please ensure your code adheres to the project's [coding conventions](https://github.com/Akhetonics/Connect-A-PIC/wiki/CONTRIBUTING#architecture-and-coding-standards) for consistency.

4. **Testing:**
    - Include unit tests for new features to maintain the project's stability and reliability.

Your contributions are greatly appreciated and will help to further develop this innovative project!

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
