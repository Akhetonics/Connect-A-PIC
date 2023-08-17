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
		[Export] protected Texture2D Texture;
		
		public StraightWaveGuideView()
		{
			Textures = new Texture2D[1, 1];
		}
		public override void _Ready()
		{
			base._Ready();
			Textures[0, 0] = Texture;
		}
	   
	}
}
