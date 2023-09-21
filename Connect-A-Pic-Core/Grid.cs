using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;

namespace CAP_Core
{
    public class Grid
    {
        public delegate void OnGridCreatedHandler(Tile[,] Tiles);
        public delegate void OnComponentChangedEventHandler(ComponentBase component, int x, int y);
        public event OnGridCreatedHandler OnGridCreated;
        public event OnComponentChangedEventHandler OnComponentPlacedOnTile;
        public event OnComponentChangedEventHandler OnComponentRemoved;
        public event OnComponentChangedEventHandler OnComponentRotated;
        public event OnComponentChangedEventHandler OnComponentMoved;
        public List<ExternalPort> ExternalPorts;

        public Tile[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int MinHeight = 10;

        public Grid(int width, int height)
        {
            if (height < MinHeight) height = MinHeight;
            Width = width;
            Height = height;
            ExternalPorts = new List<ExternalPort>() {
                    new StandardInput("io0",LightColor.Red  , 0, 2,1),
                    new StandardInput("io1",LightColor.Green, 0, 3,1),
                    new StandardInput("io2",LightColor.Blue , 0, 4,1),
                    new StandardOutput("io3",5),
                    new StandardOutput("io4",6),
                    new StandardOutput("io5",7),
                };
            GenerateAllTiles();
        }
        public List<UsedStandardInput> GetUsedStandardInputs()
        {
            List<UsedStandardInput> inputsFound = new();
            foreach (var port in ExternalPorts)
            {
                if (port is StandardInput input)
                {
                    var inputY = input.TilePositionY;
                    if (IsInGrid(0, inputY) == false) continue;
                    if (Tiles[0, inputY] == null) continue;
                    if (Tiles[0, inputY].Component == null) continue;
                    if (Tiles[0, inputY].Component.Parts[0, 0].GetPinAt(RectSide.Left) == null) continue;
                    var connectedPartOfComponent = Tiles[0, inputY].Component.GetPartAtGridXY(0, inputY);
                    Guid pinId = connectedPartOfComponent.GetPinAt(RectSide.Left).IDInFlow;
                    inputsFound.Add(new UsedStandardInput() { AttachedComponentPinId = pinId, Input = input });
                }
            }
            return inputsFound;
        }
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

        public bool IsColliding(int x, int y, int sizeX, int sizeY)
        {
            if (!IsInGrid(x, y, sizeX, sizeY))
            {
                return true;
            }

            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {
                    if (Tiles[i, j]?.Component != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool RotateComponentBy90CounterClockwise(int tileX, int tileY)
        {
            ComponentBase component = GetComponentAt(tileX, tileY);
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
        public ComponentBase? GetComponentAt(int x, int y)
        {
            if (!IsInGrid(x, y, 1, 1))
            {
                return null;
            }
            return Tiles[x, y].Component;
        }
        public ComponentBase? PlaceComponentByType(int x, int y, Type componentType)
        {
            ComponentBase component = ComponentFactory.Instance.CreateComponent(componentType);
            if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
            {
                var blockingComponent = GetComponentAt(x, y);
                throw new ComponentCannotBePlacedException(component, blockingComponent);
            }
            PlaceComponent(x, y, component);
            return component;
        }

        public void PlaceComponent(int x, int y, ComponentBase component)
        {
            if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
            {
                var blockingComponent = GetComponentAt(x, y);
                IsColliding(x, y, component.WidthInTiles, component.HeightInTiles);
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
            ComponentBase item = GetComponentAt(x, y);
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
            ComponentBase component = GetComponentAt(sourceX, sourceY);
            int oldMainGridx = component.GridXMainTile;
            int oldMainGridy = component.GridYMainTile;
            UnregisterComponentAt(component.GridXMainTile, component.GridYMainTile); // to avoid blocking itself from moving only one tile into its own subtiles
            try
            {
                PlaceComponent(x, y, component);
                OnComponentMoved?.Invoke(component, x, y);
                return true;
            }
            catch (ComponentCannotBePlacedException)
            {
                PlaceComponent(oldMainGridx, oldMainGridy, component);
            }
            return false;
        }

        public List<ParentAndChildTile> GetConnectedNeighboursOfComponent(ComponentBase component)
        {
            if (component is null) return new List<ParentAndChildTile>();
            // connectedNeighbours should get all neighbours of the component, shouldn't it?
            List<ParentAndChildTile> neighbours = new();
            for (int partX = 0; partX < component.Parts.GetLength(0); partX++)
            {
                for (int partY = 0; partY < component.Parts.GetLength(1); partY++)
                {
                    var compGridX = component.GridXMainTile + partX;
                    var compGridY = component.GridYMainTile + partY;
                    if (!IsInGrid(compGridX, compGridY, 1, 1)) continue;
                    if (component.Parts[partX, partY] == null) continue;
                    var parentTile = Tiles[component.GridXMainTile + partX, component.GridYMainTile + partY];
                    GetConnectedNeighboursOfSingleTile(parentTile)
                        .ForEach(child => neighbours.Add(new ParentAndChildTile(parentTile, child)));
                }
            }
            return neighbours;
        }
        // finds all neighbour components that are connected to a certain Tile (parent) only if the Pins match.
        public List<Tile> GetConnectedNeighboursOfSingleTile(Tile parent)
        {
            if (parent == null || parent.Component == null) return new List<Tile>();
            List<Tile> children = new();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x != 0 && y != 0 || y == 0 && x == 0) continue; // only compute Tiles that are "up down left right"
                    var neighbourX = parent.GridX + x;
                    var neighbourY = parent.GridY + y;
                    if (!IsInGrid(neighbourX, neighbourY, 1, 1)) continue;
                    Tile neighbour = Tiles[neighbourX, neighbourY];
                    var neighbourComponent = neighbour?.Component;
                    if (parent.Component == neighbourComponent || neighbourComponent == null) continue;
                    var lightDirection = new IntVector(x, y);
                    var neighbourPin = neighbour.GetPinAt(lightDirection * -1);
                    var parentPin = parent.GetPinAt(lightDirection);
                    if (neighbourPin?.MatterType != MatterType.Light || parentPin?.MatterType != MatterType.Light) continue;
                    children.Add(neighbour);
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