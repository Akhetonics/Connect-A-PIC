using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class DirectionalCouplerView : ComponentBaseView
	{
		[Export] private AnimatedSprite2D LightFlowOverlayLeftUpIn;
		private AnimatedSprite2D LightFlowOverlayLeftUpOut;
		[Export] private AnimatedSprite2D LightFlowOverlayLeftDownIn;
		private AnimatedSprite2D LightFlowOverlayLeftDownOut;
		[Export] private AnimatedSprite2D LightFlowOverlayRightUpIn;
		private AnimatedSprite2D LightFlowOverlayRightUpOut;
		[Export] private AnimatedSprite2D LightFlowOverlayRightDownIn;
		private AnimatedSprite2D LightFlowOverlayRightDownOut;

		public override void _Ready()
		{
			base._Ready();

			if (LightFlowOverlayLeftUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftUpIn)).ToString());
			if (LightFlowOverlayLeftDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftDownIn)).ToString());
			if (LightFlowOverlayRightUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightUpIn)).ToString());
			if (LightFlowOverlayRightDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightDownIn)).ToString());

			AnimationSlots.AddRange(CreateTriColorAnimSlot(0,0, RectSide.Left, LightFlowOverlayLeftUpIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(0,1, RectSide.Left, LightFlowOverlayLeftDownIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(1,0, RectSide.Right, LightFlowOverlayRightUpIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(1,1, RectSide.Right, LightFlowOverlayRightDownIn));
		}

	}
}
