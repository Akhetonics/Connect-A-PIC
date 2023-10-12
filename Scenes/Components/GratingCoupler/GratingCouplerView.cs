using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class GratingCouplerView : ComponentBaseView
	{
		[Export] protected AnimatedSprite2D LightOverlay;
		public override void _Ready()
		{
			base._Ready();
			if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is null");
			AnimationSlots = CreateTriColorAnimSlot(0, 0, RectSide.Left, LightOverlay);
		}

	}
}
