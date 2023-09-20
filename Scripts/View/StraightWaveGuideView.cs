using CAP_Core.ExternalPorts;
using Godot;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class StraightWaveGuideView : ComponentBaseView
	{
		[Export] protected Texture2D Texture;
		[Export] protected TextureRect LightOverlay;
		[Export] protected PinView LeftPin;
		[Export] protected PinView RightPin;
		
		public StraightWaveGuideView()
		{
			Textures = new Texture2D[1, 1];
		}

		public override void DisplayLightVector()
		{
			LightOverlay.Visible = true;
			var color = LightColor.Red;
			var alpha = (float)LeftPin.LightIn[color].Real;
            LightOverlay.Modulate = new Color(1, 1, 1, alpha);
        }

		public override void HideLightVector()
		{
			LightOverlay.Visible = false;
		}

		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = Texture;
		}
	   
	}
}
