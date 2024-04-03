using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles;
using static CAP_Core.Grid.GridManager;
using static CAP_Core.Grid.IComponentMover;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class ComponentMover : IComponentMover
    {
        public event OnComponentChangedEventHandler OnComponentMoved;
        public event OnComponentChangedEventHandler OnComponentRemoved;
        public event OnComponentChangedEventHandler OnComponentPlacedOnTile;
        public ITileManager TileManager { get; }

        public ComponentMover(ITileManager tileManager)
        {
            TileManager = tileManager;
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
                    TileManager.Tiles[gridX, gridY].Component = component;
                }
            }
            OnComponentPlacedOnTile?.Invoke(component, x, y);
        }

        public bool IsColliding(int x, int y, int sizeX, int sizeY, Component? exception = null)
        {
            if (!TileManager.IsInGrid(x, y, sizeX, sizeY))
            {
                return true;
            }

            for (int i = x; i < x + sizeX; i++)
            {
                for (int j = y; j < y + sizeY; j++)
                {
                    var componentInGrid = TileManager.Tiles[i, j]?.Component;
                    if (componentInGrid != null && componentInGrid != exception)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public void UnregisterComponentAt(int x, int y)
        {
            Component? component = GetComponentAt(x, y);
            if (component == null) return;
            x = component.GridXMainTile;
            y = component.GridYMainTile;
            for (int i = 0; i < component.WidthInTiles; i++)
            {
                for (int j = 0; j < component.HeightInTiles; j++)
                {
                    TileManager.Tiles[x + i, y + j].Component = null;
                }
            }
            OnComponentRemoved?.Invoke(component, x, y);
            component.ClearGridData();
        }
        public Component? GetComponentAt(int x, int y, int searchAreaWidth = 1, int searchAreaHeight = 1)
        {
            for (int i = 0; i < searchAreaWidth; i++)
            {
                for (int j = 0; j < searchAreaHeight; j++)
                {
                    int currentX = x + i;
                    int currentY = y + j;
                    if (!TileManager.IsInGrid(currentX, currentY, 1, 1)) return null;

                    var componentFound = TileManager.Tiles[currentX, currentY].Component;
                    if (componentFound != null)
                    {
                        return componentFound;
                    }
                }
            }
            return null;
        }
        public void DeleteAllComponents()
        {
            foreach (Tile tile in TileManager.Tiles)
            {
                if (tile.Component == null) continue;
                UnregisterComponentAt(tile.GridX, tile.GridY);
            }
        }
    }
}
