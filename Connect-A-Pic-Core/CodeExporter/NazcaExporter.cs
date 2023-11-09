using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using System.Text;

namespace CAP_Core.CodeExporter
{
    public class NazcaExporter : IExporter
    {
        private Grid grid;
        private List<Component.ComponentHelpers.Component> AlreadyProcessedComponents;
        private StringBuilder ExportAllConnectedTiles(Tile connectedParent, Tile child)
        {
            var nazcaString = new StringBuilder();
            nazcaString.Append(child.ExportToNazca(connectedParent));
            AlreadyProcessedComponents.Add(child.Component);
            var neighbours = GetUncomputedNeighbours(child);
            foreach (ParentAndChildTile childsNeighbourTile in neighbours)
            {
                if(AlreadyProcessedComponents.Contains(childsNeighbourTile.Child.Component)) continue;
                nazcaString.Append(ExportAllConnectedTiles(childsNeighbourTile.ParentPart, childsNeighbourTile.Child));
            }
            return nazcaString;
        }
        public string Export(Grid grid)
        {
            this.grid = grid;
            AlreadyProcessedComponents = new List<Component.ComponentHelpers.Component>();
            StringBuilder NazcaCode = new();
            NazcaCode.Append(PythonResources.CreateHeader(Resources.NazcaPDKName, Resources.NazcaStandardInputCellName));
            AddComponentsConnectedToStandardInputs(NazcaCode);
            AddOrphants(NazcaCode);
            NazcaCode.Append(PythonResources.CreateFooter());
            return NazcaCode.ToString();
        }

        private void AddComponentsConnectedToStandardInputs(StringBuilder NazcaCode)
        {
            foreach (ExternalPort port in grid.ExternalPorts)
            {
                if (port is not StandardInput input) continue;
                var x = 0;
                var y = input.TilePositionY;
                if (!grid.IsInGrid(x, y, 1, 1)) continue;
                var firstConnectedTile = grid.Tiles[x, y];
                if (firstConnectedTile.Component == null) continue;
                if (firstConnectedTile.GetPinAt(RectSide.Left)?.MatterType != MatterType.Light) continue;
                if (AlreadyProcessedComponents.Contains(firstConnectedTile.Component)) continue;
                StartConnectingAtInput(NazcaCode, input, firstConnectedTile);
            }
        }

        private void AddOrphants(StringBuilder NazcaCode)
        {
            // go through rest of components, start with one that is not being added to the NazcaCode yet
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var comp = grid.Tiles[x, y].Component;
                    if (comp == null) continue;
                    if (AlreadyProcessedComponents.Contains(comp)) continue;
                    StartConnectingAtTile(NazcaCode, grid.Tiles[x, y]);
                }
            }
        }

        private void StartConnectingAtTile(StringBuilder NazcaCode, Tile currentTile)
        {
            NazcaCode.Append(currentTile.ExportToNazcaAbsolutePosition());
            AlreadyProcessedComponents.Add(currentTile.Component);
            ExportAllNeighbours(NazcaCode, currentTile);
        }

        private void ExportAllNeighbours(StringBuilder NazcaCode, Tile currentTile)
        {
            List<ParentAndChildTile> neighbours = grid.GetConnectedNeighboursOfComponent(currentTile.Component);
            if (neighbours != null)
            {
                foreach (ParentAndChildTile neighbour in neighbours)
                {
                    if(AlreadyProcessedComponents.Contains(neighbour.Child.Component)) continue;
                    NazcaCode.Append(ExportAllConnectedTiles(neighbour.ParentPart, neighbour.Child));
                }
            }
        }

        private List<ParentAndChildTile> GetUncomputedNeighbours(Tile currentTile)
        {
            var neighbours = grid.GetConnectedNeighboursOfComponent(currentTile.Component);
            return neighbours.Where(n => !AlreadyProcessedComponents.Contains(n.Child.Component)).ToList();
        }

        private void StartConnectingAtInput(StringBuilder NazcaCode, StandardInput input, Tile firstConnectedTile)
        {
            NazcaCode.Append(firstConnectedTile.ExportToNazcaExtended(new IntVector(-1, input.TilePositionY), Resources.NazcaStandardInputCellName, input.PinName));
            AlreadyProcessedComponents.Add(firstConnectedTile.Component);
            ExportAllNeighbours(NazcaCode, firstConnectedTile);
        }
    }
}