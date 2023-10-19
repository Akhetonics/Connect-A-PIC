using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using Godot;
using System;

public partial class BendView : ComponentBaseView
{
	[Export] protected Sprite2D LightOverlay;

	public override void InitializeAnimationSlots()
	{
		if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is not assigned");
		AnimationSlots = CreateTriColorAnimSlot(0, 0, RectSide.Left, LightOverlay);
	}
}
