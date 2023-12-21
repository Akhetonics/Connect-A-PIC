using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.LightFlow;
using Components.ComponentDraftMapper.DTOs;
using System.Numerics;

namespace Components.ComponentDraftMapper
{
    public class ComponentDraftConverter
    {
        public ILogger Logger { get; }

        public ComponentDraftConverter(ILogger logger)
        {
            Logger = logger;
        }
        public List<Component> ToComponentModels(List<ComponentDraft> componentDrafts)
        {
            var modelComponents = new List<Component>();
            int typeNumber = 0;
            foreach (var draft in componentDrafts)
            {
                try
                {
                    modelComponents.Add(ToComponentModel(typeNumber, draft));
                }
                catch (Exception ex)
                {
                    Logger.PrintErr($"Exception at converting draft to Component: '{draft.identifier}' path: '{draft.sceneResPath}' msg: '{ex.Message}' ");
                }
                typeNumber++;
            }

            return modelComponents;
        }

        public static Component ToComponentModel(int typeNumber, ComponentDraft draft)
        {
            // convert PinDrafts to Model Pins
            Dictionary<(int x, int y), List<PinDraft>> PinDraftsByXY = new();
            foreach (var p in draft.pins)
            {
                if (PinDraftsByXY.ContainsKey((p.partX, p.partY)))
                {
                    PinDraftsByXY[(p.partX, p.partY)].Add(p);
                }
                else
                {
                    PinDraftsByXY.Add((p.partX, p.partY), new List<PinDraft>() { p });
                }
            }

            // Create Model Parts
            Part[,] parts = new Part[draft.widthInTiles, draft.heightInTiles];
            foreach (var pinDraft in PinDraftsByXY)
            {
                var realPins = pinDraft.Value.Select(pinDraft => new Pin(pinDraft.name, pinDraft.matterType, pinDraft.side)).ToList();
                parts[pinDraft.Key.x, pinDraft.Key.y] = new Part(realPins);
            }

            SMatrix componentConnectionsRed = GetSMatrix(draft, parts);
            return new Component(componentConnectionsRed, draft.nazcaFunctionName, draft.nazcaFunctionParameters, parts, typeNumber, DiscreteRotation.R0);
        }

        public static SMatrix GetSMatrix(ComponentDraft draft, Part[,] parts)
        {
            // Create S-Matrix Connections
            // get all real Pins
            Dictionary<Guid, Pin> ModelPins = new();
            foreach (Part part in parts)
            {
                part.Pins.ForEach(p => ModelPins.Add(p.IDInFlow, p));
                part.Pins.ForEach(p => ModelPins.Add(p.IDOutFlow, p));
            }

            Dictionary<int, PinDraft> PinDraftsByNumber = new();
            draft.pins.ForEach(p => PinDraftsByNumber.Add(p.number, p));
            List<Guid> allPinGuids = new();
            allPinGuids.AddRange(ModelPins.Values.Select(p => p.IDInFlow).Distinct());
            allPinGuids.AddRange(ModelPins.Values.Select(p => p.IDOutFlow).Distinct());

            SMatrix componentConnectionsRed = CreateSMatrix(draft.connections, parts, PinDraftsByNumber, allPinGuids, StandardWaveLengths.RedNM);
            return componentConnectionsRed;
        }

        private static SMatrix CreateSMatrix(List<Connection> connections, Part[,] parts, Dictionary<int, PinDraft> PinDraftsByNumber, List<Guid> allPinGuids, double laserWaveLengthNM)
        {
            var componentConnections = new SMatrix(allPinGuids);
            var connectionWeights = new Dictionary<(Guid, Guid), Complex>();
            foreach (Connection connectionDraft in connections)
            {
                var fromPin = PinDraftsByNumber[connectionDraft.fromPinNr];
                var toPin = PinDraftsByNumber[connectionDraft.toPinNr];
                Pin fromModelPin = GetModelPin(parts, fromPin);
                var toModelPin = GetModelPin(parts, toPin);

                var phaseShiftDegrees = PhaseShiftCalculator.GetDegrees(connectionDraft.wireLengthNM, laserWaveLengthNM);
                connectionWeights.Add((fromModelPin.IDInFlow, toModelPin.IDOutFlow), Complex.FromPolarCoordinates(connectionDraft.magnitude, phaseShiftDegrees));
            };

            componentConnections.SetValues(connectionWeights);
            return componentConnections;
        }
        private static Pin GetModelPin(Part[,] parts, PinDraft pinDraft)
        {
            return parts[pinDraft.partX, pinDraft.partY].Pins.Single(p => p.Side == pinDraft.side && p.MatterType == pinDraft.matterType);
        }
    }
}
