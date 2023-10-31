using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Helpers;
using System;
using System.IO;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class ExportNazcaCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly IExporter compiler;
        private readonly Grid grid;

        public ExportNazcaCommand(IExporter compiler, Grid grid)
        { 
            this.compiler = compiler;
            this.grid = grid;
        }
        
        public bool CanExecute(object parameter)
        {
            if( parameter is ExportNazcaParameters)
            {
                return true;
            }
            return false;
        }
        
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var nazcaParams = (ExportNazcaParameters)parameter;
            var pythonCode = compiler.Export(this.grid);
            FileSaver.SaveToFile(pythonCode, nazcaParams.Path);
            string directoryPath = Path.GetDirectoryName(nazcaParams.Path);
            string fullPath = Path.Combine(directoryPath, Resources.PDKFileName);
            FileSaver.SaveToFile(Resources.PDK, fullPath);
        }
    }

    public class ExportNazcaParameters
    {
        public ExportNazcaParameters( string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
