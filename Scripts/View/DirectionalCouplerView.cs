using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class DirectionalCouplerView : ComponentBaseView
	{
		[Export] protected Texture2D TextureLeftUp;
		[Export] protected Texture2D TextureLeftDown;
		[Export] protected Texture2D TextureRightUp;
		[Export] protected Texture2D TextureRightDown;
		
		public DirectionalCouplerView()
		{
			Textures = new Texture2D[2,2];
		}

		public override void DisplayLightVector()
		{
			throw new NotImplementedException();
		}

		public override void HideLightVector()
		{
			throw new NotImplementedException();
		}

		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = TextureLeftUp;
			Textures[0, 1] = TextureLeftDown;
			Textures[1, 0] = TextureRightUp;
			Textures[1, 1] = TextureRightDown;
		}
	   
	}
}
