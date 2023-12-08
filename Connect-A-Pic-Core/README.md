# Connect-A-PIC
Gamification of PIC Design
This Software wants to provide a very simple way of creating optical circuits on a chip that is then printable using our export to Nazca-Python function

# Target Audience
Our Target Audience is students and scientists that are interested in photonics.

# Setup Code
* Install Visual Studio 
* Install Godot Engine and add Path to your "PATH"- Environment Variable
* Clone this project into a nice folder
* Setup Launch profile for visual Studio where 
	* "Path to the executable to run" is the Path to your godot_v4.0-beta_mono_win64.exe 
	* Command Line Arguments is '--path . --verbose'
	* Working Directory is '.'
* Install Submodules using 'git submodule init' and then 'git submodule update --recursive --remote'

# Install Nazca 
In order to be able to compile the exported Nazca code, you want to have Nazca installed properly
https://nazca-design.org/installation/
