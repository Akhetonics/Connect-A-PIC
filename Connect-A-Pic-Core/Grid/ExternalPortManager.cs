using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CAP_Core.Grid
{
    public class ExternalPortManager : IExternalPortManager
    {
        public ObservableCollection<ExternalPort> ExternalPorts { get; set; }
        public ITileManager TileManager { get; }

        public ExternalPortManager(ITileManager tileManager)
        {
            TileManager = tileManager;
            ExternalPorts = new ObservableCollection<ExternalPort>() {
                    new ExternalInput("io0",LaserType.Red, 2,1),
                    new ExternalInput("io1",LaserType.Green, 3, 1),
                    new ExternalInput("io2",LaserType.Blue , 4, 1),
                    new ExternalOutput("io3",5, false),
                    new ExternalOutput("io4",6),
                    new ExternalOutput("io5",7),
                    new ExternalOutput("io6",8),
                    new ExternalOutput("io7",9),
                };
        }
        public ConcurrentBag<ExternalInput> GetAllExternalInputs()
        {
            var inputs = new ConcurrentBag<ExternalInput>();
            foreach (var p in ExternalPorts)
            {
                if (p is ExternalInput input)
                {
                    inputs.Add(input);
                }
            }
            return inputs;
        }


        public ConcurrentBag<UsedInput> GetUsedExternalInputs()
        {
            ConcurrentBag<UsedInput> inputsFound = new();
            foreach (var port in ExternalPorts)
            {
                if (port is ExternalInput input)
                {
                    var inputY = input.TilePositionY;
                    if (TileManager.IsInGrid(0, inputY) == false) continue;
                    if (TileManager.Tiles[0, inputY] == null) continue;
                    if (TileManager.Tiles[0, inputY].Component == null) continue;
                    var connectedPartOfComponent = TileManager.Tiles[0, inputY].Component.GetPartAtGridXY(0, inputY);
                    if (connectedPartOfComponent == null) continue;
                    Pin componentPin = connectedPartOfComponent.GetPinAt(RectSide.Left);
                    if (componentPin?.MatterType != MatterType.Light) continue; // if the component does not have a connected pin, then we ignore it.
                    Guid pinId = componentPin.IDInFlow;

                    inputsFound.Add(new UsedInput(input, pinId));
                }
            }
            return inputsFound;
        }
    }
}
