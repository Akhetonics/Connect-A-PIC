using CAP_Core.Component.ComponentDraftMapper;
using CAP_Core.Component.ComponentDraftMapper.DTOs;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.ComponentDrafts
{
    public class ComponentImporter
    {
        public const string ComponentFolderPath = "ref:/Scenes/Components";
        private void LoadAllPCKFiles(string startFolderPath)
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
        public static List<ComponentDraft> ImportAllComponents()
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
            } else
            {
                CustomLogger.PrintErr($"Konnte den Ordner nicht öffnen {ComponentFolderPath}");
            }

            return drafts;
            // it should go into the folder, search for all JSON files
            // import them, then search for all JSON files inside of the Scenes/Components folder that should have been populated by this
            // import the components, register the views and models so that they are visible in the toolbox window
        }
    }
}
