using ConnectAPIC.LayoutWindow.Model.Component;
using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Compiler;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class ExportNazcaCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly ICompiler compiler;
        private readonly Grid grid;

        public ExportNazcaCommand(ICompiler compiler, Grid grid)
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
            var pythonCode = compiler.Compile(this.grid);
            FileSaver.SaveToFile(pythonCode, nazcaParams.Path);
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
