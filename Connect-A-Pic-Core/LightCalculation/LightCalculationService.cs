using CAP_Core.ExternalPorts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.LightCalculation
{
    public class LightCalculationChangeEventArgs
    {
        public LightCalculationChangeEventArgs (Dictionary<Guid,Complex> lightVector, LaserType laser)
        {
            LightVector = lightVector;
            LaserInUse = laser;
        }
        public Dictionary<Guid, Complex> LightVector {get;set;}
        public LaserType LaserInUse { get; set; }
    }

    public class LightCalculationService 
    {
        public event EventHandler<LightCalculationChangeEventArgs> LightCalculationChanged;
        private Task? LightCalculationTask;
        private CancellationTokenSource CancelTokenLightCalc { get; set; } = new();
        public List<ExternalInput> LightInputs { get; }
        public ILightCalculator GridSMatrixAnalyzer { get; }

        public LightCalculationService(List<ExternalInput> lightInputs , ILightCalculator gridSMatrixAnalyzer)
        {
            LightInputs = lightInputs;
            GridSMatrixAnalyzer = gridSMatrixAnalyzer;
        }

        public async Task ShowLightPropagationAsync()
        {
            await CancelLightCalculation();
            try
            {
                 // go through all Inputs, so that the light can be turned off properly if e.g. red is turned off but some components have red light.
                foreach (var port in LightInputs)
                {
                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();
                    LightCalculationTask = Task.Run(async () =>
                    {
                        var resultLightVector = await GridSMatrixAnalyzer.CalculateLightPropagationAsync(CancelTokenLightCalc, port.LaserType.WaveLengthInNm);
                        LightCalculationChanged?.Invoke(this, new (resultLightVector, port.LaserType));
                    }, CancelTokenLightCalc.Token);
                    await LightCalculationTask.ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                LightCalculationTask = null;
            }
        }
        public async Task CancelLightCalculation()
        {
            // Cancel the ongoing calculations
            CancelTokenLightCalc.Cancel();

            try
            {
                if (LightCalculationTask != null)
                {
                    await Task.WhenAll(new List<Task>() { LightCalculationTask }).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // all Tasks are cancelled
            }
            // Dispose the old CancellationTokenSource and create a new one for the next calculations
            CancelTokenLightCalc.Dispose();
            CancelTokenLightCalc = new CancellationTokenSource();
        }
    }
}
