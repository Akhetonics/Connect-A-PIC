using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
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
            return componentDrafts.Select((draft, index) => ConvertDraftToComponent(draft, index)).ToList();
        }

        private Component ConvertDraftToComponent(ComponentDraft draft, int typeNumber)
        {
            try
            {
                var parts = CreatePartsFromDraft(draft);
                var connections = CreateConnectionsFromDraft(draft, parts);
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
                var realPins = group.Value.Select(pinDraft => new Pin(pinDraft.name, pinDraft.matterType, pinDraft.side)).ToList();
                parts[x, y] = new Part(realPins);
            }

            return parts;
        }

        private List<CAP_Core.Components.Connection> CreateConnectionsFromDraft(ComponentDraft draft, Part[,] parts)
        {
            return draft.connections.Select(dto =>
            {
                var fromPinDTO = draft.pins.Single(p => p.number == dto.fromPinNr);
                var toPinDTO = draft.pins.Single(p => p.number == dto.toPinNr);

                var fromPinModel = FindModelPin(parts, fromPinDTO);
                var toPinModel = FindModelPin(parts, toPinDTO);

                return new CAP_Core.Components.Connection
                {
                    FromPin = fromPinModel.IDInFlow,
                    ToPin = toPinModel.IDOutFlow,
                    Magnitude = dto.magnitude,
                    WireLengthNM = dto.wireLengthNM
                };
            }).ToList();
        }

        private static Pin FindModelPin(Part[,] parts, PinDraft pinDraft)
        {
            return parts[pinDraft.partX, pinDraft.partY].Pins.Single(p => p.Side == pinDraft.side && p.MatterType == pinDraft.matterType);
        }
    }
}
