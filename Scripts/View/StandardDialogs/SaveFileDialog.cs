using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
    public class SaveFileDialog
    {
        public static FileDialog Instance;
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
            Instance = new FileDialog
            {
                Mode = FileDialog.ModeEnum.Windowed,
                MinSize = new Vector2I(600, 480),
                FileMode = fileMode,
                Access = FileDialog.AccessEnum.Filesystem,
                Filters = filters.Split('|')
            };

            Instance.Connect("file_selected", Callable.From((string s) => OnFileSelected(s)));
            parent.AddChild(Instance);
            Instance.PopupCentered();
        }
    }
}
