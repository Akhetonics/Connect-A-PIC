using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public class SaveFileDialog
    {
        public static FileDialog Instance;
        public static void Open(Node parent, Action<string> OnFileSelected)
        {
            if (Instance != null) Instance.QueueFree();
            // Erstelle den Open File Dialog
            Instance = new FileDialog
            {
                Mode = FileDialog.ModeEnum.Windowed,
                MinSize = new Vector2I(600, 480),
                FileMode = FileDialog.FileModeEnum.SaveFile,
                Access= FileDialog.AccessEnum.Filesystem,
                Filters = new string[] { "*.py", "*.txt" } // Dateifilter setzen
            };

            // Verbinde das "file_selected" Signal mit einer Methode
            Instance.Connect("file_selected", Callable.From((string s)=>OnFileSelected(s)));
            parent.AddChild(Instance);
            Instance.PopupCentered();
        }
    }
}
