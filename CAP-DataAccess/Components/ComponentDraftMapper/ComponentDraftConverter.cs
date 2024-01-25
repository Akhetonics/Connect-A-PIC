using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid.FormulaReading;
using CAP_Core.Tiles.Grid;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
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
            try
            {
                var parts = CreatePartsFromDraft(draft);
                var connections = CreateWaveLengthSpecificSMatricesFromDrafts(draft, parts);
                return new Component(connections, draft.nazcaFunctionName, draft.nazcaFunctionParameters, parts, typeNumber, draft.identifier, DiscreteRotation.R0);
            }
            catch (Exception ex)
            {
                Logger.PrintErr($"Exception at converting draft to Component: '{draft.identifier}' path: '{draft.sceneResPath}' msg: '{ex.Message}' ");
                return null; // Or handle the exception as required
            }
        }
        private Part[,] CreatePartsFromDraft(ComponentDraft draft)
        {
            var parts = new Part[draft.widthInTiles, draft.heightInTiles];
            var pinsGroupedByPosition = draft.pins
                .GroupBy(p => (p.partX, p.partY))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var group in pinsGroupedByPosition)
            {
                var (x, y) = group.Key;
                var realPins = group.Value.Select(pinDraft => new Pin(pinDraft.name, pinDraft.number, pinDraft.matterType, pinDraft.side)).ToList();
                parts[x, y] = new Part(realPins);
            }

            return parts;
        }

        private Dictionary<int, SMatrix> CreateWaveLengthSpecificSMatricesFromDrafts(ComponentDraft draft, Part[,] parts)
        {
            var definedMatrices = new Dictionary<int, SMatrix>();
            var allPins = Component.GetAllPins(parts);
            var allPinsGuids = allPins.SelectMany(p => new[] { p.IDInFlow, p.IDOutFlow }).ToList();
            var pinNumberToModelMap = CreatePinNumberToModelMap(draft, parts);

            foreach (var matrixDraft in draft.sMatrices)
            {
                var matrixModel = new SMatrix(allPinsGuids);
                var connections = new Dictionary<(Guid, Guid), Complex>();
                var nonLinearConnectionFunctions = new Dictionary<(Guid, Guid), ConnectionFunction>();

                foreach (var connectionDraft in matrixDraft.connections)
                {
                    if (!pinNumberToModelMap.TryGetValue(connectionDraft.FromPinNr, out var fromPinModel))
                    {
                        Logger.PrintErr($"Kein passender Pin für 'fromPinNr' {connectionDraft.FromPinNr} gefunden.");
                        continue;
                    }
                    if (!pinNumberToModelMap.TryGetValue(connectionDraft.ToPinNr, out var toPinModel))
                    {
                        Logger.PrintErr($"Kein passender Pin für 'toPinNr' {connectionDraft.FromPinNr} gefunden.");
                        continue;
                    }
                    connections.Add((fromPinModel.IDInFlow, toPinModel.IDOutFlow), connectionDraft.ToComplexNumber());
                    if (TryGetNonLinearConnectionFunction(connectionDraft, allPins, out var connectionFunction))
                    {
                        nonLinearConnectionFunctions.Add((fromPinModel.IDInFlow, toPinModel.IDOutFlow), connectionFunction);
                    }
                }

                matrixModel.SetValues(connections);
                matrixModel.SetNonLinearConnectionFunctions(nonLinearConnectionFunctions);
                definedMatrices.Add(matrixDraft.waveLength, matrixModel);
            }

            return definedMatrices;
        }

        public static Dictionary<int, Pin> CreatePinNumberToModelMap(ComponentDraft draft, Part[,] parts)
        {
            var map = new Dictionary<int, Pin>();
            foreach (var pin in draft.pins)
            {
                var modelPin = FindModelPin(parts, pin);
                if (modelPin != null)
                {
                    map[pin.number] = modelPin;
                }
            }
            return map;
        }
        private bool TryGetNonLinearConnectionFunction(DTOs.Connection connectionDraft, List<Pin> allPins, out ConnectionFunction function)
        {
            function = default;
            if (String.IsNullOrWhiteSpace(connectionDraft.Formula) || allPins?.Count == 0) 
                return false;
            
            var convertedFunction = MathExpressionReader.ConvertToDelegate(connectionDraft.Formula, allPins);
            if (convertedFunction != null)
            {
                function = (ConnectionFunction)convertedFunction;
                return true;
            }
            return false;
        }

        private static Pin FindModelPin(Part[,] parts, PinDraft pinDraft)
        {
            return parts[pinDraft.partX, pinDraft.partY].Pins.Single(p => p.Side == pinDraft.side && p.MatterType == pinDraft.matterType);
        }
    }
}
