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
        private List<ComponentBase> AlreadyProcessedComponents;
        private StringBuilder ExportAllConnectedTiles(Tile connectedParent, Tile child)
        {
            var nazcaString = new StringBuilder();
            nazcaString.Append(child.ExportToNazca(connectedParent));
            AlreadyProcessedComponents.Add(child.Component);
            var neighbours = grid.GetConnectedNeighboursOfComponent(child.Component);
            neighbours = neighbours.Where(n => !AlreadyProcessedComponents.Contains(n.Child.Component)).ToList();
            foreach (ParentAndChildTile childsNeighbourTile in neighbours)
            {
                nazcaString.Append(ExportAllConnectedTiles(childsNeighbourTile.ParentPart, childsNeighbourTile.Child));
            }
            return nazcaString;
        }
        public string Export(Grid grid)
        {
            this.grid = grid;
            AlreadyProcessedComponents = new List<ComponentBase>();
            StringBuilder NazcaCode = new();
            NazcaCode.Append(PythonResources.CreateHeader(Resources.NazcaPDKName, Resources.NazcaStandardInputCellName));
            // start at all the three intputTiles.
            ConnectComponentsAtInputsViaPin(NazcaCode);
            NazcaCode.Append(PythonResources.CreateFooter());
            return NazcaCode.ToString();
        }

        private void ConnectComponentsAtInputsViaPin(StringBuilder NazcaCode)
        {
            foreach (ExternalPort port in grid.ExternalPorts)
            {
                if (port is not StandardInput input) continue;
                var x = 0;
                var y = input.TilePositionY;
                if (!grid.IsInGrid(x, y, 1, 1)) continue;
                var firstConnectedTile = grid.Tiles[x, y];
                if (firstConnectedTile.Component == null) continue;
                StartConnectingAtInput(NazcaCode, input, firstConnectedTile);
            }
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
            var neighbours = grid.GetConnectedNeighboursOfComponent(currentTile.Component);
            if (neighbours != null)
            {
                foreach (ParentAndChildTile neighbour in neighbours)
                {
                    NazcaCode.Append(ExportAllConnectedTiles(neighbour.ParentPart, neighbour.Child));
                }
            }
        }

        private void StartConnectingAtInput(StringBuilder NazcaCode, StandardInput input, Tile firstConnectedTile)
        {
            NazcaCode.Append(firstConnectedTile.ExportToNazcaExtended(new IntVector(-1, input.TilePositionY), Resources.NazcaStandardInputCellName, input.PinName));
            AlreadyProcessedComponents.Add(firstConnectedTile.Component);
            ExportAllNeighbours(NazcaCode, firstConnectedTile);
        }
    }
}