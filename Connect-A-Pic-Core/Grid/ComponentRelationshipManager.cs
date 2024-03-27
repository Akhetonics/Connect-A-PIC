using CAP_Core.Components;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using Component = CAP_Core.Components.Component;

namespace CAP_Core.Grid
{
    public class ComponentRelationshipManager : IComponentRelationshipManager
    {
        public ComponentRelationshipManager(ITileManager tileManager)
        {
            TileManager = tileManager;
        }

        public ITileManager TileManager { get; }

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
                    if (!TileManager.IsInGrid(compGridX, compGridY, 1, 1)) continue;
                    if (component.Parts[partX, partY] == null) continue;
                    var parentTile = TileManager.Tiles[component.GridXMainTile + partX, component.GridYMainTile + partY];
                    GetConnectedNeighborsOfSingleTile(parentTile)
                        .ForEach(child => neighbors.Add(new ParentAndChildTile(parentTile, child)));
                }
            }
            return neighbors;
        }
        // finds all neighbor components that are connected to a certain Tile (parent) only if the Pins match.
        private List<Tile> GetConnectedNeighborsOfSingleTile(Tile parent)
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
                    if (!TileManager.IsInGrid(neighborX, neighborY, 1, 1)) continue;
                    Tile neighbor = TileManager.Tiles[neighborX, neighborY];
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
}
