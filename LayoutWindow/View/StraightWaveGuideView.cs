using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class StraightWaveGuideView : ComponentBaseView
	{
		[Export] protected Texture2D TextureLeft;
		[Export] protected Texture2D TextureRight;
		
		public StraightWaveGuideView()
		{
			Textures = new Texture2D[2, 1];
		}
		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = TextureLeft;
			Textures[1, 0] = TextureRight;
		}
	   
	}
}
