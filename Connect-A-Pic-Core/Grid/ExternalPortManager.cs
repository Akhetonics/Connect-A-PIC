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
                    new ExternalInput("eio0",LaserType.Red, 2,1),
                    new ExternalInput("eio1",LaserType.Green, 3, 1),
                    new ExternalInput("eio2",LaserType.Blue , 4, 1),
                    new ExternalOutput("eio3",5),
                    new ExternalOutput("eio4",6),
                    new ExternalOutput("eio5",7),
                    new ExternalOutput("eio6",8),
                    new ExternalOutput("eio7",9),
                    new ExternalInput("wio0",LaserType.Red, 2,1, false),
                    new ExternalInput("wio1",LaserType.Green, 3, 1, false),
                    new ExternalInput("wio2",LaserType.Blue , 4, 1, false),
                    new ExternalOutput("wio3",5, false),
                    new ExternalOutput("wio4",6, false),
                    new ExternalOutput("wio5",7, false),
                    new ExternalOutput("wio6",8, false),
                    new ExternalOutput("wio7",9, false),
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
                    var inputX = port.IsLeftPort ? 0 : TileManager.Width - 1;

                    if (TileManager.IsInGrid(inputX, inputY) == false) continue;
                    if (TileManager.Tiles[inputX, inputY] == null) continue;
                    if (TileManager.Tiles[inputX, inputY].Component == null) continue;
                    var connectedPartOfComponent = TileManager.Tiles[inputX, inputY].Component.GetPartAtGridXY(inputX, inputY);
                    if (connectedPartOfComponent == null) continue;
                    Pin componentPin = connectedPartOfComponent.GetPinAt(port.IsLeftPort ? RectSide.Left : RectSide.Right);
                    if (componentPin?.MatterType != MatterType.Light) continue; // if the component does not have a connected pin, then we ignore it.
                    Guid pinId = componentPin.IDInFlow;

                    inputsFound.Add(new UsedInput(input, pinId));
                }
            }
            return inputsFound;
        }
    }
}
