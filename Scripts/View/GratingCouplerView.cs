using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class GratingCouplerView : ComponentBaseView
	{
		
		public GratingCouplerView()
		{
			
		}
		public override void _Ready()
		{
			base._Ready();
			
		}

		public override void HideLightVector()
		{
        }

        public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
        {
        }
    }
}
