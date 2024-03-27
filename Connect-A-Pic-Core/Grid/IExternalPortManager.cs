using CAP_Core.ExternalPorts;
using System.Collections.ObjectModel;

namespace CAP_Core.Grid
{
    public interface IExternalPortManager
    {
        public ObservableCollection<ExternalPort> ExternalPorts { get; }
        List<ExternalInput> GetAllExternalInputs();
        List<UsedInput> GetUsedExternalInputs();
    }
}
