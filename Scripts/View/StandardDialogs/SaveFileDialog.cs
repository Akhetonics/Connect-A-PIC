using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.IO;

namespace ConnectAPIC.LayoutWindow.View
{
    public class SaveFileDialog
    {
        public static FileDialog Instance;
        private static string DefaultDirectory =
        Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            GameManager.RepoOwnerName, GameManager.RepoName, "Exports");
        private static string PreviousDirectory = "";


        public static void Save(Node parent, Action<string> OnFileSelected, string filters = "*.py" )
        {
            OpenOrSave(parent, OnFileSelected, FileDialog.FileModeEnum.SaveFile, filters);
        }

        public static void Open(Node parent, Action<string> OnFileSelected, string filters = "*.py")
        {
            OpenOrSave(parent, OnFileSelected, FileDialog.FileModeEnum.OpenFile, filters);
        }

        private static void OpenOrSave(Node parent, Action<string> OnFileSelected, FileDialog.FileModeEnum fileMode, string filters = "*.py")
        {
            if (Instance != null) Instance.QueueFree();

            if (!Directory.Exists(DefaultDirectory))
                Directory.CreateDirectory(DefaultDirectory);

            Instance = new FileDialog
            {
                Mode = FileDialog.ModeEnum.Windowed,
                MinSize = new Vector2I(600, 480),
                FileMode = fileMode,
                Access = FileDialog.AccessEnum.Filesystem,
                Filters = filters.Split('|'),
                CurrentDir = string.IsNullOrEmpty(PreviousDirectory) ? DefaultDirectory : PreviousDirectory,
            };

            Instance.Connect("file_selected", Callable.From((string s) =>
                {
                    OnFileSelected(s);
                    PreviousDirectory = s;
                }
            ));
            parent.AddChild(Instance);
            Instance.PopupCentered();
        }
    }
}
