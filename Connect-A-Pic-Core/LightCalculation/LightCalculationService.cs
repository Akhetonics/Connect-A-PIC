using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CAP_Core.LightCalculation
{
    public interface ILightCalculationService
    {
        public event EventHandler<LightCalculationUpdated>? LightCalculationUpdated; // is only called when new data is available - not when the calculation got cancelled
        public event EventHandler CalculationCanceled;
        public Task ShowLightPropagationAsync();  
        public Task CancelLightCalculation();
    }
    public class LightCalculationService : ILightCalculationService
    {
        public event EventHandler<LightCalculationUpdated>? LightCalculationUpdated; 
        public event EventHandler CalculationCanceled;
        private Task? LightCalculationTask;
        private CancellationTokenSource CancelTokenLightCalc { get; set; } = new();
        private ConcurrentBag<ExternalInput> LightInputs { get; set; }
        private ILightCalculator GridSMatrixAnalyzer { get; }
        private SynchronizationContext? MainThreadContext { get; }

        private SemaphoreSlim Semaphore = new (1, 1);

        public LightCalculationService(GridManager grid, ILightCalculator gridSMatrixAnalyzer)
        {
            LightInputs = grid.ExternalPortManager.GetAllExternalInputs();

            grid.ExternalPortManager.ExternalPorts.CollectionChanged += async (object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                LightInputs = grid.ExternalPortManager.GetAllExternalInputs();
                if (grid.LightManager.IsLightOn)
                {
                    await ShowLightPropagationAsync();
                }
            };
            GridSMatrixAnalyzer = gridSMatrixAnalyzer;
            MainThreadContext = SynchronizationContext.Current; 
        }


        public async Task ShowLightPropagationAsync()
        {
            if (LightCalculationTask != null)
                await CancelLightCalculation();

            await Semaphore.WaitAsync();
            try
            {
                // go through all Inputs, so that the light can be turned off properly if e.g. red is turned off but some components have red light.
                foreach (var port in LightInputs)
                {
                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();

                    Dictionary<Guid, Complex> resultLightVector = new();
                    // What has to become thread safe:
                    // GridSMatrixAnalyzer
                    // LightInputs
                    LightCalculationTask = Task.Run(async () =>
                    {
                        resultLightVector = await GridSMatrixAnalyzer.CalculateFieldPropagationAsync(CancelTokenLightCalc, port.LaserType.WaveLengthInNm);
                    }, CancelTokenLightCalc.Token);
                    await LightCalculationTask.ConfigureAwait(false);

                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();

                    // the results must run in the main thread so that the UI can be updated properly
                    ExecuteOnMainThread(()=>{
                        LightCalculationUpdated?.Invoke(this, new(resultLightVector, port.LaserType));
                    });
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
        private void ExecuteOnMainThread(Action action)
        {
            if (MainThreadContext != null)
            {
                MainThreadContext.Post(_ => action(), null);
            } else
            {
                action();
            }
            
        }
        public async Task CancelLightCalculation()
        {
            if (!CancelTokenLightCalc.IsCancellationRequested)
            {
                CancelTokenLightCalc.Cancel();
            }

            await Semaphore.WaitAsync();
            try
            {
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
                CalculationCanceled?.Invoke(this, new EventArgs());
            }
        }

    }
}
