using CAP_Core.Components;
using CAP_Core.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Helpers
{
    public class GridHelpers
    {
        public static HashSet<Component> FindAllComponentsInGrid(GridManager gridManager)
        {
            HashSet<Component> components = new();
            foreach (var Tile in gridManager.TileManager.Tiles)
            {
                if (Tile.Component != null && components.Contains(Tile.Component) == false)
                {
                    components.Add(Tile.Component);
                }
            }

            return components;
        }
    }
}
