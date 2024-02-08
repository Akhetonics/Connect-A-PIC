﻿using CAP_Core.ExternalPorts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CAP_Core.LightCalculation
{
    public class LightCalculationService
    {
        public event EventHandler<LightCalculationChangeEventArgs> LightCalculationChanged;
        private Task? LightCalculationTask;
        private CancellationTokenSource CancelTokenLightCalc { get; set; } = new();
        public List<ExternalInput> LightInputs { get; }
        public ILightCalculator GridSMatrixAnalyzer { get; }
        private SemaphoreSlim Semaphore = new (1, 1);

        public LightCalculationService(List<ExternalInput> lightInputs, ILightCalculator gridSMatrixAnalyzer)
        {
            LightInputs = lightInputs;
            GridSMatrixAnalyzer = gridSMatrixAnalyzer;
        }

        public async Task ShowLightPropagationAsync()
        {
            await CancelLightCalculation();
            await Semaphore.WaitAsync();
            try
            {
                // go through all Inputs, so that the light can be turned off properly if e.g. red is turned off but some components have red light.
                foreach (var port in LightInputs)
                {
                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();
                    Dictionary<Guid, Complex> resultLightVector = new();
                    LightCalculationTask = Task.Run(async () =>
                    {
                        resultLightVector = await GridSMatrixAnalyzer.CalculateLightPropagationAsync(CancelTokenLightCalc, port.LaserType.WaveLengthInNm);
                    }, CancelTokenLightCalc.Token);
                    await LightCalculationTask.ConfigureAwait(false);
                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();
                    // the results must run in the main thread so that the UI can be updated properly
                    LightCalculationChanged?.Invoke(this, new(resultLightVector, port.LaserType));
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
                Semaphore.Release();
            }
        }

        public async Task CancelLightCalculation()
        {
            await Semaphore.WaitAsync();
            try
            {
                if (!CancelTokenLightCalc.IsCancellationRequested)
                {
                    CancelTokenLightCalc.Cancel();
                }

                if (LightCalculationTask != null)
                {
                    try
                    {
                        await Task.WhenAll(new List<Task>() { LightCalculationTask }).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // all Tasks are cancelled, handle if necessary
                    }
                }

                // Reset CTS only if it was cancelled
                if (CancelTokenLightCalc.IsCancellationRequested)
                {
                    CancelTokenLightCalc.Dispose();
                    CancelTokenLightCalc = new CancellationTokenSource();
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}