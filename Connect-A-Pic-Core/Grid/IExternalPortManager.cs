using CAP_Core.ExternalPorts;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CAP_Core.Grid
{
    public interface IExternalPortManager
    {
        public ObservableCollection<ExternalPort> ExternalPorts { get; }
        ConcurrentBag<ExternalInput> GetAllExternalInputs();
        ConcurrentBag<UsedInput> GetUsedExternalInputs();
    }
}
