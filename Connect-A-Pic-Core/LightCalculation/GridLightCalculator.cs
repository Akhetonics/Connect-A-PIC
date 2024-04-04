using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using System.Collections.Concurrent;
using System.Numerics;
namespace CAP_Core.LightCalculation
{
    public interface ILightCalculator
    {
        public Task<Dictionary<Guid, Complex>> CalculateFieldPropagationAsync(CancellationTokenSource cancelToken, int LaserWaveLengthInNm);
    }

    public class GridLightCalculator : ILightCalculator
    {
        public readonly GridManager Grid;
        public SMatrix? SystemSMatrix { get; private set; }
        public ISystemMatrixBuilder SystemMatrixBuilder { get; }

        public GridLightCalculator(ISystemMatrixBuilder systemMatrixBuilder, GridManager grid)
        {
            SystemMatrixBuilder = systemMatrixBuilder;
            Grid = grid;
        }

        // calculates the light intensity and phase at a given PIN-ID for both light-flow-directions "in" and "out" for a given period of steps
        public async Task<Dictionary<Guid, Complex>> CalculateFieldPropagationAsync(CancellationTokenSource cancelToken, int LaserWaveLengthInNm)
        {
            SystemSMatrix = SystemMatrixBuilder.GetSystemSMatrix(LaserWaveLengthInNm);
            var stepCount = SystemSMatrix.PinReference.Count() * 2;
            ConcurrentBag<UsedInput> usedInputs = Grid.ExternalPortManager.GetUsedExternalInputs()
                                 .Where(i => i.Input.LaserType.WaveLengthInNm == LaserWaveLengthInNm)
                                 .ToConcurrentBag();
            var inputVector = UsedInputConverter.ToVectorOfFields(usedInputs, SystemSMatrix);

            var fieldsAtPins = await SystemSMatrix.CalcFieldAtPinsAfterStepsAsync(inputVector, stepCount, cancelToken) ?? new Dictionary<Guid, Complex>();
            return fieldsAtPins;
        }
    }
}
