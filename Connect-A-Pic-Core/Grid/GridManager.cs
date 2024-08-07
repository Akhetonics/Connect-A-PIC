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
        public LightManager LightManager { get; }

        public GridManager(
            ITileManager tileManager,
            IComponentMover componentMover,
            IExternalPortManager externalPortManager,
            IComponentRotator componentRotator,
            IComponentRelationshipManager componentRelationshipManager,
            LightManager lightManager)
        {
            TileManager = tileManager;
            ComponentMover = componentMover;
            ExternalPortManager = externalPortManager;
            ComponentRotator = componentRotator;
            ComponentRelationshipManager = componentRelationshipManager;
            LightManager = lightManager;
        }
        public GridManager (int width , int height)
        {
            TileManager = new TileManager(width, height);
            ComponentMover = new ComponentMover(TileManager);
            ExternalPortManager = new ExternalPortManager(TileManager);
            ComponentRotator = new ComponentRotator(TileManager,ComponentMover);
            ComponentRelationshipManager = new ComponentRelationshipManager(TileManager);
            LightManager = new LightManager();
        }

    }
}
