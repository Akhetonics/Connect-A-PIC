using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;

public partial class BendView : ComponentBaseView
{
	[Export] public Texture OverlayAnimTexture { get; set; }

	public override void InitializeAnimationSlots()
	{
		this.CheckForNull(x => x.OverlayAnimTexture);
		AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, OverlayAnimTexture));
	}
}
