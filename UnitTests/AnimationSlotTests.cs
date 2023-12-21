using CAP_Core.Components;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Debuggers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class AnimationSlotTests
    {
        [Fact]
        public void CheckRotatedAnimationSlots()
        {
            AnimationSlot slot = new AnimationSlot(CAP_Core.ExternalPorts.LaserType.Red, new Godot.Vector2I(0, 0), RectSide.Right, null, null, new Godot.Vector2I(1,1));
            LightAtPin lightAtPin = new LightAtPin(0, 0, RectSide.Right, CAP_Core.ExternalPorts.LaserType.Red, new System.Numerics.Complex(1, 0), 0);
            var matchingBeforeRotation = slot.IsMatchingWithLightVector(lightAtPin);
            slot.RotateAttachedComponentCC(DiscreteRotation.R270);
            lightAtPin = new LightAtPin(0, 0, RectSide.Down, CAP_Core.ExternalPorts.LaserType.Red, new System.Numerics.Complex(1, 0), 0);
            var matchingAfterRotation = slot.IsMatchingWithLightVector(lightAtPin);

            matchingBeforeRotation.ShouldBe(true, $"the lightVector should be at the right pin. Slot: {slot.Side}, light: {lightAtPin} " );
            matchingAfterRotation.ShouldBe(true, $"the slot should be facing down now after 270° CC turn. Slot: {slot.Side}, light: {lightAtPin}");
            slot.Side.ShouldBe(RectSide.Down);
        }
    }
}
