using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class DirectionalCouplerView : ComponentBaseView
	{
		[Export] protected Texture2D TextureUp;
		[Export] protected Texture2D TextureDown;
		
		public DirectionalCouplerView()
		{
			Textures = new Texture2D[1,2];
		}
		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = TextureUp;
			Textures[0, 1] = TextureDown;
		}
	   
	}
}
