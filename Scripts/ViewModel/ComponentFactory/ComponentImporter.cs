using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper
{
    public partial class ComponentImporter : Node
    {
        public const string ComponentFolderPath = "res://Scenes/Components";
        [Export] public string[] PckFiles { get; set; }
        public void ImportPCKFiles (string[] PckFiles)
        {
            // Load all PCK files when the game runs
            foreach (var pckFile in PckFiles)
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
        public static List<ComponentDraft> ReadComponentJSONDrafts()
        {
            var globalCompPath = ProjectSettings.GlobalizePath(ComponentFolderPath);
            List<ComponentDraft> drafts = new();
            if (Directory.Exists(globalCompPath))
            {
                foreach (var filePath in Directory.EnumerateFiles(globalCompPath, "*.json", SearchOption.AllDirectories))
                {
                    try
                    {
                        var draft = ComponentDraftFileReader.Read(filePath);
                        drafts.Add(draft);
                    } catch (Exception ex)
                    {
                        CustomLogger.PrintEx(ex);
                        continue;
                    }
                }
            }
            else
            {
                CustomLogger.PrintErr($"Fldr doesn't exist: {ComponentFolderPath}");
            }

            return drafts;
        }
    }
}
