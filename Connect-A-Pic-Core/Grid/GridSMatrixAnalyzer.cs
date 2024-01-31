using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using System.Linq;
using System.Numerics;
namespace CAP_Core.Tiles.Grid
{
    public class GridSMatrixAnalyzer
    {
        public readonly GridManager Grid;
        public Dictionary<(Guid, Guid), Complex>? InterComponentConnections { get; private set; }
        public SMatrix? SystemSMatrix { get; private set; }
        public int LaserWaveLengthInNm { get; }

        public GridSMatrixAnalyzer(GridManager grid, int laserWaveLengthInNm)
        {
            Grid = grid;
            LaserWaveLengthInNm = laserWaveLengthInNm;
        }

        // calculates the light intensity and phase at a given PIN-ID for both light-flow-directions "in" and "out" for a given period of steps
        public async Task<Dictionary<Guid, Complex>> CalculateLightPropagationAsync()
        {
            return await Task.Run(() =>
            {
                UpdateSystemSMatrix();
                var stepCount = SystemSMatrix.PinReference.Count() * 2;
                var usedInputs = Grid.GetUsedExternalInputs()
                                     .Where(i => i.Input.LaserType.WaveLengthInNm == LaserWaveLengthInNm)
                                     .ToList();
                var inputVector = UsedInputConverter.ToVector(usedInputs, SystemSMatrix);

                return SystemSMatrix.GetLightPropagation(inputVector, stepCount) ?? new Dictionary<Guid, Complex>();
            });
        }

        private void UpdateSystemSMatrix()
        {
            var allComponentsSMatrices = GetAllComponentsSMatrices((int)LaserWaveLengthInNm);
            SMatrix allConnectionsSMatrix = CreateInterComponentsConnectionsMatrix();
            allComponentsSMatrices.Add(allConnectionsSMatrix);
            SystemSMatrix = SMatrix.CreateSystemSMatrix(allComponentsSMatrices);
        }

        public SMatrix CreateInterComponentsConnectionsMatrix()
        {
            if (InterComponentConnections == null || InterComponentConnections.Count <= 0)
            {
                CalcAllConnectionsBetweenComponents();
            }
            var allUsedPinIDs = InterComponentConnections.SelectMany(c => new[] { c.Key.Item1, c.Key.Item2 }).Distinct().ToList();
            Grid.GetUsedExternalInputs().ForEach(input => allUsedPinIDs.Add(input.AttachedComponentPinId)); // Grating coupler has no internal connections and might be only connected to the Laser directly
            var allConnectionsSMatrix = new SMatrix(allUsedPinIDs, new());
            allConnectionsSMatrix.SetValues(InterComponentConnections);
            return allConnectionsSMatrix;
        }

        public List<SMatrix> GetAllComponentsSMatrices(int waveLength)
        {
            var allComponents = Grid.GetAllComponents();
            var allSMatrices = new List<SMatrix>();
            foreach (var component in allComponents)
            {
                if (component.WaveLengthToSMatrixMap.TryGetValue(waveLength, out var matrixFound))
                {
                    allSMatrices.Add(matrixFound);
                }
                else
                {
                    throw new InvalidDataException($"The Matrix was not defined for the specific waveLength: {waveLength} at component {component.Identifier}");
                }
            };
            return allSMatrices;
        }
        private void CalcAllConnectionsBetweenComponents()
        {
            int gridWidth = Grid.Tiles.GetLength(0);
            int gridHeight = Grid.Tiles.GetLength(1);
            InterComponentConnections = new();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    ConnectAllBorderEdgesOfComponentAt(x, y);
                }
            }
        }

        private void ConnectAllBorderEdgesOfComponentAt(int x, int y)
        {
            Array allSides = Enum.GetValues(typeof(RectSide));
            foreach (RectSide side in allSides)
            {
                IntVector offset = side;
                if (Grid.Tiles[x, y].Component == null) continue;
                if (!Grid.IsInGrid(x + offset.X, y + offset.Y)) continue;
                var foreignTile = Grid.Tiles[x + offset.X, y + offset.Y];
                if (!IsComponentBorderEdge(x, y, foreignTile)) continue;
                Pin currentPin = Grid.Tiles[x, y].GetPinAt(side);
                if (currentPin == null) continue;
                var foreignPinSide = offset * -1;
                Pin foreignPin = foreignTile.GetPinAt(foreignPinSide);
                if (foreignPin == null) continue;

                InterComponentConnections.Add((currentPin.IDOutFlow, foreignPin.IDInFlow), 1);
            }
        }

        private bool IsComponentBorderEdge(int gridX, int gridY, Tile foreignTile)
        {
            if (foreignTile == null) return false;
            var centeredComponent = Grid.Tiles[gridX, gridY].Component;
            return centeredComponent != foreignTile.Component;
        }

        public override string ToString() => new GridSMatrixPrinter(this).ToString();
    }
}
