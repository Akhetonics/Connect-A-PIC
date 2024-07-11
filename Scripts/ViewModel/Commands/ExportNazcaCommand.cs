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
            if (!CanExecute(parameter)) return;

            var nazcaParams = (ExportNazcaParameters)parameter;
            var pythonCode = compiler.Export(this.grid);

            FileStream fileStream = new FileStream(nazcaParams.Path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            Task pythonCodeTask = this.dataAccessor.Write(nazcaParams.Path, pythonCode);

            string directoryPath = Path.GetDirectoryName(nazcaParams.Path);
            string fullPDKPath = Path.Combine(directoryPath, Resources.PDKFileName);
            Task pdkTask = this.dataAccessor.Write(fullPDKPath, Resources.PDK);

            try
            {
                await pdkTask;
                await pythonCodeTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (!pythonCodeTask.IsCompletedSuccessfully || !pdkTask.IsCompletedSuccessfully)
                throw new Exception("Writing failed");
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
