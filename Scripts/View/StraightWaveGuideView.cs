using CAP_Core.ExternalPorts;
using Godot;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class StraightWaveGuideView : ComponentBaseView
	{
		
		[Export] protected TextureRect LightOverlay;
		[Export] protected PinView LeftPin;
		[Export] protected PinView RightPin;
		
		public StraightWaveGuideView()
		{
			
		}

		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			foreach (LightAtPin light in lightsAtPins)
			{
				LightOverlay.Visible = true;
				var color = LightColor.Red;
				var alpha = (float)LeftPin.LightIn[color].Real;
				LightOverlay.Modulate = new Color(1, 1, 1, alpha);
			}
			
		}

		public override void HideLightVector()
		{
			LightOverlay.Visible = false;
		}

		public override void _Ready()
		{
			base._Ready();
			
		}
	   
	}
}
