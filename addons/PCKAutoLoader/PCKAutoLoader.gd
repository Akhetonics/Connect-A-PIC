# Remember to enable 'tool' mode for the script to run in the editor
@tool
extends EditorPlugin

var componentImporter
var button

func _enter_tree():
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
	var pckPaths : Array = find_all_pck_components_in_global_path(folderPath)
	# Set the PCKLoader's exported property
	var root = get_editor_interface().get_edited_scene_root()
	componentImporter = root.find_child("ComponentImporter", true, false)
	componentImporter.set("PckFilesConcatenated",";".join(pckPaths))
	print ("PCKs found: " + "\n  >".join(pckPaths))

func find_all_pck_components_in_global_path(startFolderPath):
	var globalPCKPaths = []
	var systemFolderPath = ProjectSettings.globalize_path(startFolderPath)
	
	if DirAccess.dir_exists_absolute(systemFolderPath):
		var dir = DirAccess.open(systemFolderPath)
		if  dir != null:
			dir.list_dir_begin()
			var filePath = dir.get_next()
			while filePath != "":
				var fullFileName = dir.get_current_dir(true) + "/" + filePath
				var globalFilePath = ProjectSettings.localize_path(fullFileName)
				
				# add pck file if it was found
				if filePath.ends_with(".pck"): 
					globalPCKPaths.append(globalFilePath)
					
				# run the whole search for every subfolder that was found as well
				if filePath.contains(".") == false:
					var subfolderPcks = find_all_pck_components_in_global_path(globalFilePath)
					globalPCKPaths.append_array(subfolderPcks)
				filePath = dir.get_next()
			dir.list_dir_end()
	else:
		print("Couldn't open folder: " + systemFolderPath)

	return globalPCKPaths
