using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.FormulaReading;
using CAP_Core.Grid.FormulaReading;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using MathNet.Numerics;
using System.Data;
using System.Globalization;
using System.Numerics;

namespace CAP_DataAccess.Components.ComponentDraftMapper
{
    public class ComponentDraftConverter
    {
        public ILogger Logger { get; }

        public ComponentDraftConverter(ILogger logger)
        {
            CultureInfo englishCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = englishCulture;
            Thread.CurrentThread.CurrentUICulture = englishCulture;
            Logger = logger;
        }

        public List<Component> ToComponentModels(List<ComponentDraft> componentDrafts)
        {
            return componentDrafts.Select((draft, index) => ConvertDraftToComponent(draft, index)).ToList();
        }

        private Component ConvertDraftToComponent(ComponentDraft draft, int typeNumber)
        {
            if (draft == null) throw new InvalidOperationException($"The parameter {nameof(draft)} cannot be null");
            try
            {
                var parts = CreatePartsFromDraft(draft);
                var wavelengthToMatrixMap = CreateWaveLengthSpecificSMatricesFromDrafts(draft, parts);
                var sliderMap = CreateSliderMap(draft);
                return new Component(wavelengthToMatrixMap, sliderMap, draft.NazcaFunctionName, draft.NazcaFunctionParameters, parts, typeNumber, draft.Identifier, DiscreteRotation.R0);
            }
            catch (Exception ex)
            {
                Logger.PrintErr($"Exception at converting draft to Component: '{draft.Identifier}' path: '{draft.SceneResPath}' msg: '{ex.Message}' ");
                throw ex;
            }
        }
        private Part[,] CreatePartsFromDraft(ComponentDraft draft)
        {
            var parts = new Part[draft.WidthInTiles, draft.HeightInTiles];
            var pinsGroupedByPosition = draft.Pins
                .GroupBy(p => (p.PartX, p.PartY))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var group in pinsGroupedByPosition)
            {
                var (x, y) = group.Key;
                var realPins = group.Value.Select(pinDraft => new Pin(pinDraft.Name, pinDraft.Number, pinDraft.MatterType, pinDraft.Side)).ToList();
                parts[x, y] = new Part(realPins);
            }

            return parts;
        }

        private Dictionary<int, SMatrix> CreateWaveLengthSpecificSMatricesFromDrafts(ComponentDraft draft, Part[,] parts)
        {
            var definedMatrices = new Dictionary<int, SMatrix>();
            var allPins = Component.GetAllPins(parts);
            var allSliders = CreateSliderMap(draft);
            var allSliderGuids = allSliders.Select(s=>s.ID).ToList();
            var allPinsGuids = allPins.SelectMany(p => new[] { p.IDInFlow, p.IDOutFlow }).ToList();
            var pinNumberToModelMap = CreatePinNumberToModelMap(draft, parts);

            foreach (var matrixDraft in draft.SMatrices)
            {
                var matrixModel = new SMatrix(allPinsGuids, allSliderGuids);
                (var nonLinearConnections , var linearConnections) = CreateConnecitons(allPins, allSliders, pinNumberToModelMap, matrixDraft);
                matrixModel.SetValues(linearConnections);
                matrixModel.NonLinearConnections = nonLinearConnections;
                definedMatrices.Add(matrixDraft.WaveLength, matrixModel);
            }

            return definedMatrices;
        }

        private ( Dictionary<(Guid, Guid), ConnectionFunction> NonLinearConnections , Dictionary<(Guid,Guid),Complex> LinearConnections) 
            CreateConnecitons(List<Pin> allPins, List<Slider> allSliders ,Dictionary<int, Pin> pinNumberToModelMap, WaveLengthSpecificSMatrix matrixDraft)
        {
            var connections = new Dictionary<(Guid, Guid), Complex>();
            var nonLinearConnectionFunctions = new Dictionary<(Guid, Guid), ConnectionFunction>();

            foreach (var connectionDraft in matrixDraft.Connections)
            {
                if (!pinNumberToModelMap.TryGetValue(connectionDraft.FromPinNr, out var fromPinModel))
                {
                    Logger.PrintErr($"unable to find suitable pin for 'fromPinNr' {connectionDraft.FromPinNr}.");
                    continue;
                }
                if (!pinNumberToModelMap.TryGetValue(connectionDraft.ToPinNr, out var toPinModel))
                {
                    Logger.PrintErr($"unable to find suitable pin for 'toPinNr' {connectionDraft.FromPinNr}.");
                    continue;
                }
                connections.Add((fromPinModel.IDInFlow, toPinModel.IDOutFlow), connectionDraft.ToComplexNumber());
                if (TryGetNonLinearConnectionFunction(connectionDraft, allPins, allSliders, out var connectionFunction))
                {
                    nonLinearConnectionFunctions.Add((fromPinModel.IDInFlow, toPinModel.IDOutFlow), connectionFunction);
                }
            }

            return (nonLinearConnectionFunctions , connections);
        }

        public static Dictionary<int, Pin> CreatePinNumberToModelMap(ComponentDraft draft, Part[,] parts)
        {
            var map = new Dictionary<int, Pin>();
            foreach (var pin in draft.Pins)
            {
                var modelPin = FindModelPin(parts, pin);
                if (modelPin != null)
                {
                    map[pin.Number] = modelPin;
                }
            }
            return map;
        }
        private bool TryGetNonLinearConnectionFunction(DTOs.Connection connectionDraft, List<Pin> allPins, List<Slider> allSliders, out ConnectionFunction function)
        {
            function = default;
            if (String.IsNullOrWhiteSpace(connectionDraft.Formula) || allPins?.Count == 0) 
                return false;
            
            var convertedFunction = MathExpressionReader.ConvertToDelegate(connectionDraft.Formula, allPins, allSliders);
            if (convertedFunction != null)
            {
                function = (ConnectionFunction)convertedFunction;
                return true;
            }
            return false;
        }

        private static List< Slider> CreateSliderMap(ComponentDraft draft)
        {
            var sliderIdToValueMap = new List<Slider>();
            if (draft?.Sliders != null)
            {
                foreach (var slider in draft.Sliders)
                {
                    sliderIdToValueMap.Add(new Slider(Guid.NewGuid(), slider.SliderNumber, (slider.MinVal + slider.MaxVal) / 2, slider.MaxVal, slider.MinVal));
                }
            }
            return sliderIdToValueMap;
        }
        private static Pin FindModelPin(Part[,] parts, PinDraft pinDraft)
        {
            return parts[pinDraft.PartX, pinDraft.PartY].Pins.Single(p => p.Side == pinDraft.Side && p.MatterType == pinDraft.MatterType);
        }
    }
}
