using static CAP_Core.Grid.GridManager;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public interface IComponentMover
    {
        public delegate void OnComponentChangedEventHandler(Component component, int x, int y);
        public event OnComponentChangedEventHandler OnComponentMoved;
        public event OnComponentChangedEventHandler OnComponentRemoved;
        public event OnComponentChangedEventHandler OnComponentPlacedOnTile;
        void PlaceComponent(int x, int y, Component component);
        bool IsColliding(int x, int y, int sizeX, int sizeY, Component? exception = null);
        void UnregisterComponentAt(int x, int y);
        Component? GetComponentAt(int x, int y, int searchAreaWidth = 1, int searchAreaHeight = 1);
        void DeleteAllComponents();
    }
}
