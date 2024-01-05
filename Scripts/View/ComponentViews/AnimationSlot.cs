using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ConnectAPIC.LayoutWindow.View
{
    public class AnimationSlot
    {
        public AnimationSlot(LaserType color, Vector2I tileOffsetXY, RectSide side, Sprite2D baseOverlaySprite, Texture texture, Vector2I componentSizeInTiles)
        {
            if (componentSizeInTiles.X < 1) throw new WrongSizeException("component Width must be at least 1");
            if (componentSizeInTiles.Y < 1) throw new WrongSizeException("component Height must be at least 1");
            this.MatchingLaser = color;
            TileOffset = tileOffsetXY;
            Side = side;
            BaseOverlaySprite = baseOverlaySprite;
            this.Texture = texture;
            ComponentSizeInTiles = componentSizeInTiles;
        }

        /// <summary>
        /// Determines if the current object's properties match with those of the provided LightAtPin object.
        /// </summary>
        /// <param name="lightVector">LightAtPin object to compare.</param>
        /// <returns>
        /// A tuple with a boolean indicating if a match is found and an Exception detailing the mismatch reason if any.
        /// The Exception is null for a successful match.
        /// </returns>
        /// <remarks>
        /// Mismatch is checked for X and Y tile offsets, 'Side', and laser wavelength in nm.
        /// Specific exceptions are returned for each type of mismatch.
        /// </remarks>
        public (bool isMatching, Exception misMatchReason) IsMatchingWithLightVector(LightAtPin lightVector)
        {
            if (TileOffset.X != lightVector.partOffsetX) return (false, new OffsetWrongException($"X = {TileOffset.X}, but lightVector: {lightVector.partOffsetX}"));
            if (TileOffset.Y != lightVector.partOffsetY) return (false, new OffsetWrongException($"Y = {TileOffset.Y}, but lightVector: {lightVector.partOffsetY}"));
            if (Side != lightVector.side) return (false, new SideNotMatchingException());
            if (MatchingLaser.WaveLengthInNm != lightVector.lightType.WaveLengthInNm) return (false, new WaveLengthNotMatchingException("laserWaveLength: " + MatchingLaser.WaveLengthInNm));
            return (true, null);
        }
        public static List<AnimationSlot> FindMatching(List<AnimationSlot> slots, LightAtPin lightVector)
        {
            return slots.Where(s => s.IsMatchingWithLightVector(lightVector).isMatching).ToList();
        }
        private Vector2I RotateOffsetBy90CounterClockwise(Vector2I offset)
        {
            return new Vector2I(offset.Y, ComponentSizeInTiles.X - 1 - offset.X);
        }
        public void RotateAttachedComponentCC(DiscreteRotation targetRotationCC)
        {
            var cycles = this.Rotation.CalculateCyclesTillTargetRotation(targetRotationCC);
            for (int i = 0; i < cycles; i++)
            {
                RotateAttachedComponentBy90CC();
            }
        }
        public void RotateAttachedComponentBy90CC()
        {
            Rotation = Rotation.RotateBy90CounterC();
            Side = Side.RotateSideCounterClockwise(DiscreteRotation.R90);
            TileOffset = RotateOffsetBy90CounterClockwise(TileOffset);
        }

        public LaserType MatchingLaser { get; }
        public Vector2I TileOffset { get; private set; }
        public DiscreteRotation Rotation { get; private set; }
        public RectSide Side { get; private set; }
        public Sprite2D BaseOverlaySprite { get; }
        public Texture Texture { get; }
        public Vector2I ComponentSizeInTiles { get; }
        public static int MaxShaderAnimationSlots { get; } = 8; // the amount of texture-slots that the shader can handle.
    }
    public class WrongSizeException : Exception
    {
        public WrongSizeException() { }
        public WrongSizeException(string message) : base(message) { }
    }
    public class WaveLengthNotMatchingException : Exception
    {
        public WaveLengthNotMatchingException() { }
        public WaveLengthNotMatchingException(string message) : base(message) { }
    }
    public class SideNotMatchingException : Exception
    {
        public SideNotMatchingException() { }
        public SideNotMatchingException(string message) : base(message) { }
    }
    public class OffsetWrongException : Exception
    {
        public OffsetWrongException() { }
        public OffsetWrongException(string message) : base(message) { }
    }
}
