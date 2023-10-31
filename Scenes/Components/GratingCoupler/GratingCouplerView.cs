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
		[Export] protected Texture OverlayAnimTexture;
		public override void InitializeAnimationSlots()
		{
			this.CheckForNull(x => x.OverlayAnimTexture);
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left,OverlayAnimTexture));
		}
	}
}
