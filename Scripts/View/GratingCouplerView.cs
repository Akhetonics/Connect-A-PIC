using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class GratingCouplerView : ComponentBaseView
	{
		[Export] protected Texture2D Texture;
		
		public GratingCouplerView()
		{
			Textures = new Texture2D[1, 1];
		}
		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = Texture;
		}

		public override void DisplayLightVector()
		{

		}

		public override void HideLightVector()
		{
			throw new NotImplementedException();
		}
	}
}
