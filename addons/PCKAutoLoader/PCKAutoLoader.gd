# Remember to enable 'tool' mode for the script to run in the editor
@tool
extends EditorPlugin

var componentImporter
var button

func _enter_tree():
	# Find or create the ComponentImporter node in the scene
	var root = get_editor_interface().get_edited_scene_root()
	# Create and add the custom button
	button = Button.new()
	button.text = "root: " + root.name
	var componentImporter = root.find_child("ComponentImporter", true, false)

	# Create and add the custom button
	button = Button.new()
	button.text = "Fetch All PCK"
	button.connect("pressed", Callable( self, "_on_Button_pressed"))
	add_control_to_container(CONTAINER_TOOLBAR, button)
func _exit_tree():
	remove_control_from_container(CONTAINER_TOOLBAR, button)
	button.free()
	
func _on_Button_pressed():
	# Define the folder path where the PCK files are located
	var folderPath = "res://Scenes/Components/"
	# Find all PCK files in the folder
	var pckPaths = find_all_pck_components_in_global_path(folderPath)
	# Set the PCKLoader's exported property
	componentImporter.set("pckFiles",pckPaths)

func find_all_pck_components_in_global_path(startFolderPath):
	var globalPCKPaths = []
	var systemFolderPath = ProjectSettings.globalize_path(startFolderPath)
	
	if DirAccess.dir_exists_absolute(systemFolderPath):
		var dir = DirAccess.new
		if dir.open(systemFolderPath) == OK:
			dir.list_dir_begin()
			var filePath = dir.get_next()
			while filePath != "":
				if filePath.ends_with(".pck"):
					var globalFilePath = ProjectSettings.localize_path(dir.get_current_dir().plus_file(filePath))
					globalPCKPaths.append(globalFilePath)
					print("found: " + globalFilePath)
				filePath = dir.get_next()
			dir.list_dir_end()

	else:
		print("Couldn't open folder: " + systemFolderPath)

	return globalPCKPaths
