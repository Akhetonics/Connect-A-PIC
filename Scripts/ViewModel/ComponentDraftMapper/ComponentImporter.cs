using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System.Collections.Generic;
using System.IO;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
    public class ComponentImporter
    {
        public const string ComponentFolderPath = "ref:/Scenes/Components";
        public static void CopyAllComponentsFromPCKtoJson(string startFolderPath)
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
                        CustomLogger.PrintLn($"PCK geladen: {globalFilePath}");
                    }
                    else
                    {
                        CustomLogger.PrintErr($"Fehler beim Laden von PCK: {globalFilePath}");
                    }
                }
            }
            else
            {
                CustomLogger.PrintErr($"Konnte den Ordner nicht öffnen: {startFolderPath}");
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
                CustomLogger.PrintErr($"Konnte den Ordner nicht öffnen {ComponentFolderPath}");
            }

            return drafts;
            // it should go into the folder, search for all PCK files
            // import them, then search for all JSON files inside of the Scenes/Components folder that should have been populated by this
            // import the components, register the views and models so that they are visible in the toolbox window
        }
    }
}
