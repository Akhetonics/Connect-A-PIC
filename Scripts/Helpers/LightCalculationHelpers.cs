using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ConnectAPIC.Scripts.Helpers
{
    internal static class LightCalculationHelpers
    {

        public static List<LightAtPin> ConvertToLightAtPins(Dictionary<Guid, Complex> lightFieldVector, LaserType laserType, Component componentModel)
        {
            List<LightAtPin> lightAtPins = new();

            for (int offsetX = 0; offsetX < componentModel.WidthInTiles; offsetX++)
            {
                for (int offsetY = 0; offsetY < componentModel.HeightInTiles; offsetY++)
                {
                    var part = componentModel.GetPartAt(offsetX, offsetY);
                    foreach (var localSide in Enum.GetValues(typeof(RectSide)).OfType<RectSide>())
                    {
                        var pin = part.GetPinAt(localSide);
                        if (pin == null) continue;
                        var lightFlow = new LightAtPin(
                            offsetX,
                            offsetY,
                            localSide,
                            laserType,
                            lightFieldVector.TryGetVal(pin.IDInFlow),
                            lightFieldVector.TryGetVal(pin.IDOutFlow)
                            );
                        lightAtPins.Add(lightFlow);
                    }
                }
            }

            return lightAtPins;
        }
    }
}
