using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class DirectionalCouplerView : ComponentBaseView
	{
		[Export] private Texture LightFlowOverlayLeftUpIn;
		[Export] private Texture LightFlowOverlayLeftDownIn;
		[Export] private Texture LightFlowOverlayRightUpIn;
		[Export] private Texture LightFlowOverlayRightDownIn;

		public override void InitializeAnimationSlots()
		{
			this.CheckForNull(x => x.LightFlowOverlayLeftUpIn);
			this.CheckForNull(x => x.LightFlowOverlayLeftDownIn);
			this.CheckForNull(x => x.LightFlowOverlayRightUpIn);
			this.CheckForNull(x => x.LightFlowOverlayRightDownIn);
			var textOffsetX = GameManager.TilePixelSize;
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, LightFlowOverlayLeftUpIn));
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, LightFlowOverlayLeftDownIn,0,1));
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, LightFlowOverlayRightUpIn,1,0,new Vector2()));
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, LightFlowOverlayRightDownIn));
		}

	}
}
