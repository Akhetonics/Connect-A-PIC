using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Debuggers;
using Godot;
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

            matchingBeforeRotation.isMatching.ShouldBe(true, $"the lightVector should be at the right pin. Slot: {slot.Side}, light: {lightAtPin} " );
            matchingAfterRotation.isMatching.ShouldBe(true, $"the slot should be facing down now after 270° CC turn. Slot: {slot.Side}, light: {lightAtPin}");
            slot.Side.ShouldBe(RectSide.Down);
        }
        [Fact]
        public void TestMismatchInXOffset()
        {
            AnimationSlot slot = new(
                LaserType.Red,
                new Vector2I(1, 0), // Intentional mismatch in X offset
                RectSide.Right,
                null,
                null,
                new Vector2I(1, 1));

            LightAtPin lightAtPin = new LightAtPin(
                0, // Different X offset
                0,
                RectSide.Right,
                LaserType.Red,
                new System.Numerics.Complex(1, 0),
                0);

            var matchResult = slot.IsMatchingWithLightVector(lightAtPin);

            matchResult.isMatching.ShouldBe(false, "X offsets do not match, should not be a match");
            matchResult.misMatchReason.ShouldBeOfType<OffsetWrongException>("Mismatch due to X offset should throw OffsetWrongException");
        }

        [Fact]
        public void TestMismatchInYOffset()
        {
            // Creating an AnimationSlot with a specific Y offset
            AnimationSlot slot = new AnimationSlot(
                LaserType.Red,
                new Vector2I(0, 1), // Intentional mismatch in Y offset
                RectSide.Right,
                null,
                null,
                new Vector2I(1, 1));

            // Creating a LightAtPin with a different Y offset
            LightAtPin lightAtPin = new LightAtPin(
                0,
                0, // Different Y offset
                RectSide.Right,
                LaserType.Red,
                new System.Numerics.Complex(1, 0),
                0);

            // Testing the IsMatchingWithLightVector method
            var matchResult = slot.IsMatchingWithLightVector(lightAtPin);

            // Asserting that there is no match and the reason is an OffsetWrongException
            matchResult.isMatching.ShouldBe(false, "Y offsets do not match, should not be a match");
            matchResult.misMatchReason.ShouldBeOfType<OffsetWrongException>("Mismatch due to Y offset should throw OffsetWrongException");
        }

        [Fact]
        public void TestMismatchInSide()
        {
            // Creating an AnimationSlot with a specific Side
            AnimationSlot slot = new AnimationSlot(
                LaserType.Red,
                new Vector2I(0, 0),
                RectSide.Right, // Intentional mismatch in Side
                null,
                null,
                new Vector2I(1, 1));

            // Creating a LightAtPin with a different Side
            LightAtPin lightAtPin = new LightAtPin(
                0,
                0,
                RectSide.Left, // Different Side
                LaserType.Red,
                new System.Numerics.Complex(1, 0),
                0);

            // Testing the IsMatchingWithLightVector method
            var matchResult = slot.IsMatchingWithLightVector(lightAtPin);

            // Asserting that there is no match and the reason is a SideNotMatchingException
            matchResult.isMatching.ShouldBe(false, "Sides do not match, should not be a match");
            matchResult.misMatchReason.ShouldBeOfType<SideNotMatchingException>("Mismatch due to Side should throw SideNotMatchingException");
        }
        [Fact]
        public void TestMismatchInLaserWavelength()
        {
            // Creating an AnimationSlot with a specific laser wavelength
            AnimationSlot slot = new AnimationSlot(
                LaserType.Red,
                new Vector2I(0, 0),
                RectSide.Right,
                null,
                null,
                new Vector2I(1, 1));

            // Creating a LightAtPin with a different laser wavelength
            LightAtPin lightAtPin = new LightAtPin(
                0,
                0,
                RectSide.Right,
                LaserType.Green, // Green light with another wavelength
                new System.Numerics.Complex(1, 0),
                0);

            // Testing the IsMatchingWithLightVector method
            var matchResult = slot.IsMatchingWithLightVector(lightAtPin);

            // Asserting that there is no match and the reason is a WaveLengthNotMatchingException
            matchResult.isMatching.ShouldBe(false, "Laser wavelengths do not match, should not be a match");
            matchResult.misMatchReason.ShouldBeOfType<WaveLengthNotMatchingException>("Mismatch due to laser wavelength should throw WaveLengthNotMatchingException");
        }
    }
}
