using CAP_Contracts;
using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class ExportNazcaCommand : CommandBase<ExportNazcaParameters>
    {
        public event EventHandler CanExecuteChanged;
        private readonly IExporter compiler;
        private readonly GridManager grid;
        private readonly IDataAccessor dataAccessor;

        public ExportNazcaCommand(IExporter compiler, GridManager grid, IDataAccessor dataAccessor)
        { 
            this.compiler = compiler;
            this.grid = grid;
            this.dataAccessor = dataAccessor;
        }
        
        internal async override Task ExecuteAsyncCmd(ExportNazcaParameters parameter)
        {
            var nazcaParams = (ExportNazcaParameters)parameter;
            var pythonCode = compiler.Export(this.grid);
            await this.dataAccessor.Write(nazcaParams.Path, pythonCode);
            string directoryPath = Path.GetDirectoryName(nazcaParams.Path);
            string fullPDKPath = Path.Combine(directoryPath, Resources.PDKFileName);
            await this.dataAccessor.Write(fullPDKPath, Resources.PDK);
            if (!CanExecute(parameter)) return;
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
