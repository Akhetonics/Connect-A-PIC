using CAP_Contracts;
using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using System.Text;

namespace CAP_Core.CodeExporter
{
    public class NazcaExporter : IExporter
    {
        private GridManager? grid;
        private List<Component>? AlreadyProcessedComponents = new();
        private StringBuilder ExportAllConnectedTiles(Tile connectedParent, Tile child)
        {
            if (AlreadyProcessedComponents == null) throw new NullReferenceException($"The list of {nameof(AlreadyProcessedComponents)} cannot be null");
            if (child.Component == null) throw new NullReferenceException($"child.{nameof(child.Component)} cannot be null");
            var nazcaString = new StringBuilder();
            nazcaString.Append(child.ExportToNazca(connectedParent));
            AlreadyProcessedComponents.Add(child.Component);
            var neighbors = GetUnComputedNeighbors(child);
            foreach (ParentAndChildTile childNeighborTile in neighbors)
            {
                if(AlreadyProcessedComponents.Contains(childNeighborTile.Child.Component)) continue;
                nazcaString.Append(ExportAllConnectedTiles(childNeighborTile.ParentPart, childNeighborTile.Child));
            }
            return nazcaString;
        }
        public string Export(GridManager grid)
        {
            this.grid = grid;
            AlreadyProcessedComponents = new List<Component>();
            StringBuilder NazcaCode = new();
            NazcaCode.Append(PythonResources.CreateHeader(Resources.NazcaPDKName, Resources.NazcaStandardLeftInputCellName, Resources.NazcaStandardRightInputCellName));
            AddComponentsConnectedToStandardInputs(NazcaCode);
            AddOrphans(NazcaCode);
            NazcaCode.Append(PythonResources.CreateFooter());
            return NazcaCode.ToString();
        }

        private void AddComponentsConnectedToStandardInputs(StringBuilder NazcaCode)
        {
            foreach (ExternalPort port in grid.ExternalPortManager.ExternalPorts)
            {
                if (port is not ExternalInput input) continue;
                var x = input.IsLeftPort ? 0 : grid.TileManager.Width - 1;
                var y = input.TilePositionY;
                if (!grid.TileManager.IsInGrid(x, y, 1, 1)) continue;
                var firstConnectedTile = grid.TileManager.Tiles[x, y];
                if (firstConnectedTile.Component == null) continue;
                if (firstConnectedTile.GetPinAt(input.IsLeftPort ? RectSide.Left : RectSide.Right)?.MatterType != MatterType.Light) continue;
                if (AlreadyProcessedComponents.Contains(firstConnectedTile.Component)) continue;
                StartConnectingAtInput(NazcaCode, input, firstConnectedTile);
            }
        }

        private void AddOrphans(StringBuilder NazcaCode)
        {
            // go through rest of components, start with one that is not being added to the NazcaCode yet
            for (int x = 0; x < grid.TileManager.Width; x++)
            {
                for (int y = 0; y < grid.TileManager.Height; y++)
                {
                    var comp = grid.TileManager.Tiles[x, y].Component;
                    if (comp == null) continue;
                    if (AlreadyProcessedComponents.Contains(comp)) continue;
                    StartConnectingAtTile(NazcaCode, grid.TileManager.Tiles[x, y]);
                }
            }
        }

        private void StartConnectingAtTile(StringBuilder NazcaCode, Tile currentTile)
        {
            NazcaCode.Append(currentTile.ExportToNazcaAbsolutePosition());
            AlreadyProcessedComponents.Add(currentTile.Component);
            ExportAllNeighbors(NazcaCode, currentTile);
        }

        private void ExportAllNeighbors(StringBuilder NazcaCode, Tile currentTile)
        {
            List<ParentAndChildTile> neighbors = grid.ComponentRelationshipManager.GetConnectedNeighborsOfComponent(currentTile.Component);
            if (neighbors != null)
            {
                foreach (ParentAndChildTile neighbor in neighbors)
                {
                    if(AlreadyProcessedComponents.Contains(neighbor.Child.Component)) continue;
                    NazcaCode.Append(ExportAllConnectedTiles(neighbor.ParentPart, neighbor.Child));
                }
            }
        }

        private List<ParentAndChildTile> GetUnComputedNeighbors(Tile currentTile)
        {
            var neighbors = grid.ComponentRelationshipManager.GetConnectedNeighborsOfComponent(currentTile.Component);
            return neighbors.Where(n => !AlreadyProcessedComponents.Contains(n.Child.Component)).ToList();
        }

        private void StartConnectingAtInput(StringBuilder NazcaCode, ExternalInput input, Tile firstConnectedTile)
        {
            NazcaCode.Append(firstConnectedTile
                .ExportToNazcaExtended(new IntVector(input.IsLeftPort ? -1 : grid.TileManager.Width, input.TilePositionY), input.IsLeftPort ? Resources.NazcaStandardLeftInputCellName : Resources.NazcaStandardRightInputCellName, input.PinName));
            AlreadyProcessedComponents.Add(firstConnectedTile.Component);
            ExportAllNeighbors(NazcaCode, firstConnectedTile);
        }
    }
}
