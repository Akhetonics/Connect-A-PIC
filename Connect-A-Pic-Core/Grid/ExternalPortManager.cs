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
                    new ExternalOutput("io1",3),
                    //new ExternalInput("io1",LaserType.Green, 3, 1),
                    new ExternalInput("io2",LaserType.Blue , 4, 1),
                    new ExternalOutput("io3",5),
                    new ExternalOutput("io4",6),
                    new ExternalOutput("io5",7),
                    new ExternalOutput("io6",8),
                    new ExternalOutput("io7",9),
                    //TODO: how will io naming work for exporting? needs to be discussed
                    new ExternalInput("io8",LaserType.Red, 2,1, false),
                    new ExternalInput("io9",LaserType.Green, 3, 1, false),
                    new ExternalInput("io10",LaserType.Blue , 4, 1, false),
                    new ExternalOutput("io11",5, false),
                    new ExternalOutput("io12",6, false),
                    new ExternalOutput("io13",7, false),
                    new ExternalOutput("io14",8, false),
                    new ExternalOutput("io15",9, false),
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
