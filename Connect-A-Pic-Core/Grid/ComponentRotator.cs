using CAP_Core.Components.ComponentHelpers;
using static CAP_Core.Grid.GridManager;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class ComponentRotator : IComponentRotator
    {
        public event IComponentMover.OnComponentChangedEventHandler OnComponentRotated;
        public ITileManager TileManager { get; }
        public IComponentMover ComponentMover { get; }
        public ComponentRotator(ITileManager tileManager, IComponentMover componentMover)
        {
            TileManager = tileManager;
            ComponentMover = componentMover;
        }

        public bool RotateComponentBy90CounterClockwise(int tileX, int tileY)
        {
            Component? component = ComponentMover.GetComponentAt(tileX, tileY);
            if (component == null) return false;
            var tile = TileManager.Tiles[tileX, tileY];
            if (tile == null || tile.Component == null) return false;

            var rotatedComponent = tile.Component;
            int x = tile.Component.GridXMainTile;
            int y = tile.Component.GridYMainTile;
            ComponentMover.UnregisterComponentAt(tile.GridX, tile.GridY);
            rotatedComponent.RotateBy90CounterClockwise();
            try
            {
                ComponentMover.PlaceComponent(x, y, rotatedComponent);
                OnComponentRotated?.Invoke(rotatedComponent, tileX, tileY);
                return true;
            }
            catch (ComponentCannotBePlacedException)
            {
                rotatedComponent.Rotation90CounterClock--;
                ComponentMover.PlaceComponent(x, y, rotatedComponent);
                return false;
            }
        }
    }
}
