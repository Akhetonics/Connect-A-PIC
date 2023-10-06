using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
    public class AnimationSlot
	{
		public AnimationSlot(LightColor color, int offsetX, int offsetY, RectSide side, AnimatedSprite2D overlayIn , AnimatedSprite2D overlayOut) {
			this.Color = color;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
			Side = side;
			this.OverlayInFlow = overlayIn;
			this.OverlayOutFlow = overlayOut;
		}
		public bool IsMatchingWithLightVector(LightAtPin lightVector, bool ignoreSide = false)
		{
			if (OffsetX != lightVector.partOffsetX) return false;
			if (OffsetY != lightVector.partOffsetY) return false;
			if (ignoreSide == false && Side != lightVector.side) return false;
			if (Color != lightVector.color) return false;
			return true;
		}
		public static AnimationSlot FindMatching(List<AnimationSlot> slots, LightAtPin lightVector, bool ignoreSide = false)
		{
			try
			{
				return slots.Single(s => s.IsMatchingWithLightVector(lightVector, ignoreSide));
			}
			catch (Exception ex)
			{
				CustomLogger.PrintErr(ex.Message);
				throw;
			}
		}
		public LightColor Color { get; }
		public int OffsetX { get; }
		public int OffsetY { get; }
		public RectSide Side { get; }
		public AnimatedSprite2D OverlayInFlow { get; }
		public AnimatedSprite2D OverlayOutFlow { get; }
	}
}
