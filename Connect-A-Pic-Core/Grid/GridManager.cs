using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using System.ComponentModel;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class GridManager
    {
        public delegate void OnGridCreatedHandler(Tile[,] Tiles);
        public delegate void OnComponentChangedEventHandler(Component component, int x, int y);
        public event OnGridCreatedHandler OnGridCreated;
        public event OnComponentChangedEventHandler OnComponentPlacedOnTile;
        public event OnComponentChangedEventHandler OnComponentRemoved;
        public event OnComponentChangedEventHandler OnComponentRotated;
        public event OnComponentChangedEventHandler OnComponentMoved;
        public event EventHandler<bool> OnLightSwitched;
        public List<ExternalPort> ExternalPorts;
        private bool isLightOn;
        public bool IsLightOn
        {
            get => isLightOn; set
            {
                isLightOn = value; OnLightSwitched?.Invoke(this, value);
            }
        }
        public Tile[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int MinHeight = 10;

        public GridManager(int width, int height)
        {
            if (height < MinHeight) height = MinHeight;
            Width = width;
            Height = height;
            ExternalPorts = new List<ExternalPort>() {
                    new ExternalInput("io0",LaserType.Red, 2,1),
                    new ExternalInput("io1",LaserType.Green, 3, 1),
                    new ExternalInput("io2",LaserType.Blue , 4, 1),
                    new ExternalOutput("io3",5),
                    new ExternalOutput("io4",6),
                    new ExternalOutput("io5",7),
                    new ExternalOutput("io6",8),
                    new ExternalOutput("io7",9),
                };
            GenerateAllTiles();
        }
        public List<UsedInput> GetUsedExternalInputs()
        {
            List<UsedInput> inputsFound = new();
            foreach (var port in ExternalPorts)
            {
                if (port is ExternalInput input)
                {
                    var inputY = input.TilePositionY;
                    if (IsInGrid(0, inputY) == false) continue;
                    if (Tiles[0, inputY] == null) continue;
                    if (Tiles[0, inputY].Component == null) continue;
                    var connectedPartOfComponent = Tiles[0, inputY].Component.GetPartAtGridXY(0, inputY);
                    if (connectedPartOfComponent == null) continue;
                    Pin componentPin = connectedPartOfComponent.GetPinAt(RectSide.Left);
                    if (componentPin?.MatterType != MatterType.Light) continue; // if the component does not have a connected pin, then we ignore it.
                    Guid pinId = componentPin.IDInFlow;

                    inputsFound.Add(new UsedInput(input, pinId));
                }
            }
            return inputsFound;
        }
        public List<ExternalInput> GetAllExternalInputs() => 
            ExternalPorts
            .Where(p => p is ExternalInput)
            .Select(p=>(ExternalInput)p)
            .ToList();

        private void GenerateAllTiles()
        {
            Tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new Tile(x, y);
                }
            }
            OnGridCreated?.Invoke(Tiles);
        }

        public bool IsColliding(int x, int y, int sizeX, int sizeY, Component? exception = null)
        {
            if (!IsInGrid(x, y, sizeX, sizeY))
            {
                return true;
            }

            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {
                    var componentInGrid = Tiles[i, j]?.Component;
                    if (componentInGrid != null && componentInGrid != exception)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool RotateComponentBy90CounterClockwise(int tileX, int tileY)
        {
            Component? component = GetComponentAt(tileX, tileY);
            if (component == null) return false;
            var tile = Tiles[tileX, tileY];
            if (tile == null || tile.Component == null) return false;

            var rotatedComponent = tile.Component;
            int x = tile.Component.GridXMainTile;
            int y = tile.Component.GridYMainTile;
            UnregisterComponentAt(tile.GridX, tile.GridY);
            rotatedComponent.RotateBy90CounterClockwise();
            try
            {
                PlaceComponent(x, y, rotatedComponent);
                OnComponentRotated?.Invoke(rotatedComponent, tileX, tileY);
                return true;
            }
            catch (ComponentCannotBePlacedException)
            {
                rotatedComponent.Rotation90CounterClock--;
                PlaceComponent(x, y, rotatedComponent);
                return false;
            }
        }
        public bool IsInGrid(int x, int y, int width = 1, int height = 1)
        {
            return x >= 0 && y >= 0 && x + width <= Width && y + height <= Height;
        }
        public Component? GetComponentAt(int x, int y, int searchAreaWidth = 1, int searchAreaHeight = 1)
        {
            for (int i = 0; i < searchAreaWidth; i++)
            {
                for (int j = 0; j < searchAreaHeight; j++)
                {
                    int currentX = x + i;
                    int currentY = y + j;
                    if (!IsInGrid(currentX, currentY, 1, 1)) return null;

                    var componentFound = Tiles[currentX, currentY].Component;
                    if (componentFound != null)
                    {
                        return componentFound;
                    }
                }
            }
            return null;
        }
        public List<Component> GetAllComponents()
        {
            List<Component> components = new();
            foreach (Tile tile in Tiles)
            {
                if (tile.Component == null) continue;
                components.Add(tile.Component);
            }
            return components.Distinct().ToList();
        }
        public void DeleteAllComponents()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.Component == null) continue;
                UnregisterComponentAt(tile.GridX, tile.GridY);
            }
        }
        public void PlaceComponent(int x, int y, Component component)
        {
            if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
            {
                var blockingComponent = GetComponentAt(x, y);
                throw new ComponentCannotBePlacedException(component, blockingComponent);
            }
            component.RegisterPositionInGrid(x, y);
            for (int i = 0; i < component.WidthInTiles; i++)
            {
                for (int j = 0; j < component.HeightInTiles; j++)
                {
                    int gridX = x + i;
                    int gridY = y + j;
                    Tiles[gridX, gridY].Component = component;
                }
            }
            OnComponentPlacedOnTile?.Invoke(component, x, y);
        }
        public void UnregisterComponentAt(int x, int y)
        {
            Component? item = GetComponentAt(x, y);
            if (item == null) return;
            x = item.GridXMainTile;
            y = item.GridYMainTile;
            for (int i = 0; i < item.WidthInTiles; i++)
            {
                for (int j = 0; j < item.HeightInTiles; j++)
                {
                    Tiles[x + i, y + j].Component = null;
                }
            }
            OnComponentRemoved?.Invoke(item, x, y);
            item.ClearGridData();
        }
        public bool MoveComponent(int x, int y, int sourceX, int sourceY)
        {
            Component? component = GetComponentAt(sourceX, sourceY);
            if (component == null) return false;
            int oldMainGridX = component.GridXMainTile;
            int oldMainGridY = component.GridYMainTile;
            UnregisterComponentAt(component.GridXMainTile, component.GridYMainTile); // to avoid blocking itself from moving only one tile into its own subTiles
            try
            {
                PlaceComponent(x, y, component);
                OnComponentMoved?.Invoke(component, x, y);
                return true;
            }
            catch (ComponentCannotBePlacedException)
            {
                PlaceComponent(oldMainGridX, oldMainGridY, component);
            }
            return false;
        }

        public List<ParentAndChildTile> GetConnectedNeighborsOfComponent(Component component)
        {
            if (component is null) return new List<ParentAndChildTile>();
            List<ParentAndChildTile> neighbors = new();
            for (int partX = 0; partX < component.Parts.GetLength(0); partX++)
            {
                for (int partY = 0; partY < component.Parts.GetLength(1); partY++)
                {
                    var compGridX = component.GridXMainTile + partX;
                    var compGridY = component.GridYMainTile + partY;
                    if (!IsInGrid(compGridX, compGridY, 1, 1)) continue;
                    if (component.Parts[partX, partY] == null) continue;
                    var parentTile = Tiles[component.GridXMainTile + partX, component.GridYMainTile + partY];
                    GetConnectedNeighborsOfSingleTile(parentTile)
                        .ForEach(child => neighbors.Add(new ParentAndChildTile(parentTile, child)));
                }
            }
            return neighbors;
        }
        // finds all neighbor components that are connected to a certain Tile (parent) only if the Pins match.
        public List<Tile> GetConnectedNeighborsOfSingleTile(Tile parent)
        {
            if (parent == null || parent.Component == null) return new List<Tile>();
            List<Tile> children = new();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x != 0 && y != 0 || y == 0 && x == 0) continue; // only compute Tiles that are "up down left right"
                    var neighborX = parent.GridX + x;
                    var neighborY = parent.GridY + y;
                    if (!IsInGrid(neighborX, neighborY, 1, 1)) continue;
                    Tile neighbor = Tiles[neighborX, neighborY];
                    var neighborComponent = neighbor?.Component;
                    if (parent.Component == neighborComponent || neighborComponent == null) continue;
                    var lightDirection = new IntVector(x, y);
                    var neighborPin = neighbor.GetPinAt(lightDirection * -1);
                    var parentPin = parent.GetPinAt(lightDirection);
                    if (neighborPin?.MatterType != MatterType.Light || parentPin?.MatterType != MatterType.Light) continue;
                    children.Add(neighbor);
                }
            }
            return children;
        }
    }
    public record struct ParentAndChildTile(Tile ParentPart, Tile Child)
    {
        public static implicit operator (Tile, Tile)(ParentAndChildTile value)
        {
            return (value.ParentPart, value.Child);
        }

        public static implicit operator ParentAndChildTile((Tile, Tile) ParentAndChild)
        {
            return new ParentAndChildTile(ParentAndChild.Item1, ParentAndChild.Item2);
        }
    }
}