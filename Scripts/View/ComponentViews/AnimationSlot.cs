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

namespace ConnectAPIC.LayoutWindow.View
{
    public class AnimationSlot
	{
		public AnimationSlot(LaserType color, Vector2I tileOffsetXY, RectSide side, Sprite2D baseOverlaySprite, Texture texture , Vector2I componentSizeInTiles ) {
			if (componentSizeInTiles.X < 1) throw new WrongSizeException("component Width must be at least 1");
			if (componentSizeInTiles.Y < 1) throw new WrongSizeException("component Height must be at least 1");
			this.MatchingLaser = color;
            TileOffset = tileOffsetXY;
			Side = side;
            BaseOverlaySprite = baseOverlaySprite;
            this.Texture = texture;
            ComponentSizeInTiles = componentSizeInTiles;
        }
		public bool IsMatchingWithLightVector(LightAtPin lightVector )
		{
			if (TileOffset.X != lightVector.partOffsetX) return false;
			if (TileOffset.Y != lightVector.partOffsetY) return false;
			if (Side != lightVector.side) return false;
			if (MatchingLaser.WaveLengthInNm != lightVector.lightType.WaveLengthInNm) return false;
			return true;
		}
		public static AnimationSlot TryFindMatching(List<AnimationSlot> slots, LightAtPin lightVector)
		{
			try
			{
				return slots.SingleOrDefault(s => s.IsMatchingWithLightVector(lightVector));
			}
			catch (Exception ex)
			{
				GameManager.Instance?.Logger?.PrintErr( "Error at TryFindMatching slots - Could not find lightVector: " + lightVector.ToString() + " " +  ex.Message);
				throw;
			}
		}
        private Vector2I RotateOffsetBy90Clockwise(Vector2I offset)
        {
            return new Vector2I(ComponentSizeInTiles.Y - 1 - offset.Y, offset.X);
        }
        private Vector2I RotateOffsetBy90CounterClockwise(Vector2I offset)
        {
            return new Vector2I(offset.Y, ComponentSizeInTiles.X - 1 - offset.X);
        }
        public void RotateAttachedComponentCC(DiscreteRotation targetRotationCC ) {
            var cycles = this.Rotation.CalculateCyclesTillTargetRotation(targetRotationCC);
            for (int i = 0; i < cycles; i++)
			{
				RotateAttachedComponentBy90CC();
            }
        }
		public void RotateAttachedComponentBy90CC ()
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
    }
    public class WrongSizeException : Exception
    {
        public WrongSizeException() { }
        public WrongSizeException(string message) : base(message) { }
    }
}
