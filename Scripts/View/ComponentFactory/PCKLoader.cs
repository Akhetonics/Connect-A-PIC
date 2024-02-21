using CAP_Contracts.Logger;
using CAP_DataAccess.Helpers;
using Godot;

namespace ConnectAPIC.Scripts.View.ComponentFactory
{

    public class PCKLoader
    {
        public FileFinder GodotFileFinder { get; private set; }
        public string ComponentFolderPath { get; }
        public ILogger Logger { get; }

        public PCKLoader(string componentFolderPath, ILogger logger)
        {
            GodotFileFinder = new(new DirectoryAccessGodot());
            ComponentFolderPath = componentFolderPath;
            Logger = logger;
        }
        public void LoadStandardPCKs()
        {
            var pckFiles = GodotFileFinder.FindRecursively(ComponentFolderPath, "pck");
            // Load all PCK files when the game runs
            foreach (var pckFile in pckFiles)
            {
                if (ProjectSettings.LoadResourcePack(pckFile))
                {
                    Logger.Print($"PCK loaded successfully: {pckFile}");
                }
                else
                {
                    Logger.PrintErr($"Error while loading PCK: {pckFile}");
                }
            }
        }
    }
}
