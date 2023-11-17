using CAP_DataAccess.Helpers;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
using Godot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectAPIC.Scripts.View.ComponentFactory
{

    public partial class PCKLoader
    {
        public FileFinder GodotFileFinder { get; private set; }
        public string ComponentFolderPath { get; }

        public PCKLoader(string componentFolderPath)
        {
            GodotFileFinder = new(new DirectoryAccessGodot());
            ComponentFolderPath = componentFolderPath;
        }
        public void LoadStandardPCKs()
        {
            var pckFiles = GodotFileFinder.FindRecursively(ComponentFolderPath, "pck");
            // Load all PCK files when the game runs
            foreach (var pckFile in pckFiles)
            {
                if (ProjectSettings.LoadResourcePack(pckFile))
                {
                    Logger.Inst.PrintLn($"PCK loaded successfully: {pckFile}");
                }
                else
                {
                    Logger.Inst.PrintErr($"Error while loading PCK: {pckFile}");
                }
            }
        }
    }
}
