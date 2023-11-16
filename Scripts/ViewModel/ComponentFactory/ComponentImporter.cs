using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
	public partial class ComponentImporter : Node
	{
		public const string ComponentFolderPath = "res://Scenes/Components";
		public string[] PckFiles
		{
			get => PckFilesConcatenated.Split(";");
		}
		[Export] public string PckFilesConcatenated;

		public void ImportInternalPCKFiles ()
		{
			var pckFiles = FindFilesRecursively(ComponentFolderPath, "pck");
			// Load all PCK files when the game runs
			foreach (var pckFile in pckFiles)
			{
				if (ProjectSettings.LoadResourcePack(pckFile))
				{
					CustomLogger.PrintLn($"PCK loaded successfully: {pckFile}");
				}
				else
				{
					CustomLogger.PrintErr($"Error while loading PCK: {pckFile}");
				}
			}
		}

		public static List<string> FindFilesRecursively(string path, string extensionPattern)
		{
			var paths = new List<string>();
			using var dir = DirAccess.Open(path);
			if (dir != null)
			{
				dir.ListDirBegin();
				string fileName = dir.GetNext();
				while (fileName != "")
				{
					if (dir.CurrentIsDir())
					{
						GD.Print($"Found directory: {fileName}");
						paths.AddRange(FindFilesRecursively(path + "/" + fileName, extensionPattern));
					}
					else
					{
						if(fileName.EndsWith(extensionPattern, StringComparison.OrdinalIgnoreCase))
						{
							paths.Add(path + "/" + fileName);
							GD.Print($"Found file: {fileName}");
						}
					}
					fileName = dir.GetNext();
				}
			}
			else
			{
				GD.Print("An error occurred when trying to access the path.");
			}
			return paths;
		}
		
		public static List<ComponentDraft> ReadComponentJSONDrafts()
		{
			return FindFilesRecursively(ComponentFolderPath, "json")
				.Select(file => ComponentDraftFileReader.TryRead(file))
				.ToList();
		}
	}
}
