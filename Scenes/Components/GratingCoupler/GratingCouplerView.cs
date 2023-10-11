using CAP_Core.ExternalPorts;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class GratingCouplerView : ComponentBaseView
	{
		[Export] protected AnimatedSprite2D LightOverlay;
		public GratingCouplerView()
		{
			
		}
		public override void _Ready()
		{
			base._Ready();
			if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is null");
			
		}
		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			var lightIntensity = lightsAtPins.First(l => l.color == LightColor.Red).lightInFlow;
			LightOverlay.Visible = true;
			LightOverlay.Play();
			var alpha = (float)lightIntensity.Real;
			LightOverlay.Modulate = new Color(1, 1, 1, alpha);
		}

		public override void HideLightVector()
		{
			LightOverlay.Visible = false;
		}
	}
}
