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
		[Export] private Sprite2D LightFlowOverlayLeftUpIn;
		private Sprite2D LightFlowOverlayLeftUpOut;
		[Export] private Sprite2D LightFlowOverlayLeftDownIn;
		private Sprite2D LightFlowOverlayLeftDownOut;
		[Export] private Sprite2D LightFlowOverlayRightUpIn;
		private Sprite2D LightFlowOverlayRightUpOut;
		[Export] private Sprite2D LightFlowOverlayRightDownIn;
		private Sprite2D LightFlowOverlayRightDownOut;

		public override void InitializeAnimationSlots()
		{
			if (LightFlowOverlayLeftUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftUpIn)).ToString());
			if (LightFlowOverlayLeftDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftDownIn)).ToString());
			if (LightFlowOverlayRightUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightUpIn)).ToString());
			if (LightFlowOverlayRightDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightDownIn)).ToString());

			AnimationSlots = new();
			AnimationSlots.AddRange(CreateTriColorAnimSlot(0,0, RectSide.Left, LightFlowOverlayLeftUpIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(0,1, RectSide.Left, LightFlowOverlayLeftDownIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(1,0, RectSide.Right, LightFlowOverlayRightUpIn));
			AnimationSlots.AddRange(CreateTriColorAnimSlot(1,1, RectSide.Right, LightFlowOverlayRightDownIn));
		}

	}
}
