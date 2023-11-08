using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System.Collections.Generic;
using System.IO;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
    public class ComponentImporter
    {
        public const string ComponentFolderPath = "ref:/Scenes/Components";
        public static void ImportAllPCKComponents(string startFolderPath)
        {
            // Convert Godot to system Paths
            var systemFolderPath = ProjectSettings.GlobalizePath(startFolderPath);

            if (Directory.Exists(systemFolderPath))
            {
                foreach (var filePath in Directory.EnumerateFiles(systemFolderPath, "*.pck", SearchOption.AllDirectories))
                {
                    var globalFilePath = ProjectSettings.LocalizePath(filePath);
                    if (ProjectSettings.LoadResourcePack(globalFilePath))
                    {
                        CustomLogger.PrintLn($"PCK loaded successfully: {globalFilePath}");
                    }
                    else
                    {
                        CustomLogger.PrintErr($"Error while loading PCK: {globalFilePath}");
                    }
                }
            }
            else
            {
                CustomLogger.PrintErr($"Could not open folder: {startFolderPath}");
            }
        }
        public static List<ComponentDraft> ImportAllJsonComponents()
        {
            var globalCompPath = ProjectSettings.LocalizePath(ComponentFolderPath);
            List<ComponentDraft> drafts = new();
            if (Directory.Exists(globalCompPath))
            {
                foreach (var filePath in Directory.EnumerateFiles(globalCompPath, "*.json", SearchOption.AllDirectories))
                {
                    var draft = ComponentDraftFileReader.Read(filePath);
                    drafts.Add(draft);
                }
            }
            else
            {
                CustomLogger.PrintErr($"Could not open folder: {ComponentFolderPath}");
            }

            return drafts;
        }
    }
}
