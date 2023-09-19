using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
