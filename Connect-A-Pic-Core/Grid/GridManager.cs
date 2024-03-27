using CAP_Core.Tiles;
using System.ComponentModel;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class GridManager
    {
        public delegate void OnGridCreatedHandler(Tile[,] Tiles);
        public ITileManager TileManager { get; set; }
        public IComponentMover ComponentMover { get; }
        public IExternalPortManager ExternalPortManager { get; }
        public IComponentRotator ComponentRotator { get; }
        public IComponentRelationshipManager ComponentRelationshipManager { get; }

        public GridManager(ITileManager tileManager, IComponentMover componentMover, IExternalPortManager externalPortManager, IComponentRotator componentRotator, IComponentRelationshipManager componentRelationshipManager)
        {
            TileManager = tileManager;
            ComponentMover = componentMover;
            ExternalPortManager = externalPortManager;
            ComponentRotator = componentRotator;
            ComponentRelationshipManager = componentRelationshipManager;
        }

    }
}
