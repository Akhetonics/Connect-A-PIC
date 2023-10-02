using CAP_Core.ExternalPorts;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class DirectionalCouplerView : ComponentBaseView
	{
		[Export] private AnimatedSprite2D LightFlowOverlayLeftUpIn;
		private AnimatedSprite2D LightFlowOverlayLeftUpOut;
		[Export] private AnimatedSprite2D LightFlowOverlayLeftDownIn;
		private AnimatedSprite2D LightFlowOverlayLeftDownOut;
		[Export] private AnimatedSprite2D LightFlowOverlayRightUpIn;
		private AnimatedSprite2D LightFlowOverlayRightUpOut;
		[Export] private AnimatedSprite2D LightFlowOverlayRightDownIn;
		private AnimatedSprite2D LightFlowOverlayRightDownOut;

		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			LightFlowOverlayLeftUpIn.Visible = true;
			LightFlowOverlayLeftDownIn.Visible = true;
			LightFlowOverlayRightUpIn.Visible = true;
			LightFlowOverlayRightDownIn.Visible = true;

			LightFlowOverlayLeftUpOut.Visible = true;
			LightFlowOverlayLeftDownOut.Visible = true;
			LightFlowOverlayRightUpOut.Visible = true;
			LightFlowOverlayRightDownOut.Visible = true;
			var lightLeftUp = lightsAtPins.Where(l => l.partOffsetX == 0 && l.partOffsetY == 0);
			foreach(var light in lightLeftUp)
			{
				// we have to match the partOffsetX, partoffsetY with the Left/Up values here, in theory also the side should be matched,
				// then for all 3 colors we want to modulate the realvalue.
				// then the phase should change the animation state
				// we want to create inflow and outflow where outflow changes the animation to run backwards
				LightFlowOverlayLeftUpIn.Modulate = new Color(light.color.ToGodotColor(), (float)light.lightInFlow.Real);
				LightFlowOverlayLeftUpIn.FrameProgress = light.lightInFlow.NormalizePhase();
				LightFlowOverlayLeftUpOut.Modulate = new Color(light.color.ToGodotColor(), (float)light.lightOutFlow.Real);
				LightFlowOverlayLeftUpOut.FrameProgress = 1-(float)light.lightInFlow.Phase;
				LightFlowOverlayLeftUpOut.SpeedScale *= -1;
			}
		}

		public override void HideLightVector()
		{
			LightFlowOverlayLeftUpIn.Visible = false;
			LightFlowOverlayLeftDownIn.Visible = false;
			LightFlowOverlayRightUpIn.Visible = false;
			LightFlowOverlayRightDownIn.Visible = false;

			LightFlowOverlayLeftUpOut.Visible = false;
			LightFlowOverlayLeftDownOut.Visible = false;
			LightFlowOverlayRightUpOut.Visible = false;
			LightFlowOverlayRightDownOut.Visible = false;
			// todo: Here I really need to create a list<Overlays> that know where they are located so they can automatically be assigned to the right LightAtPin Element.
		}

		public override void _Ready()
		{
			base._Ready();
			if (LightFlowOverlayLeftUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftUpIn)).ToString());
			if (LightFlowOverlayLeftDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayLeftDownIn)).ToString());
			if (LightFlowOverlayRightUpIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightUpIn)).ToString());
			if (LightFlowOverlayRightDownIn == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(LightFlowOverlayRightDownIn)).ToString());

			LightFlowOverlayLeftUpOut = LightFlowOverlayLeftUpIn.Duplicate() as AnimatedSprite2D;
            LightFlowOverlayLeftDownOut = LightFlowOverlayLeftDownIn.Duplicate() as AnimatedSprite2D;
            LightFlowOverlayRightUpOut = LightFlowOverlayRightUpIn.Duplicate() as AnimatedSprite2D;
            LightFlowOverlayRightDownOut = LightFlowOverlayRightDownIn.Duplicate() as AnimatedSprite2D;
		}
		public override ComponentBaseView Duplicate()
		{
			var copy = base.Duplicate() as DirectionalCouplerView;

            if (LightFlowOverlayLeftUpIn == null) CustomLogger.PrintErr((nameof(LightFlowOverlayRightDownIn)).ToString());
            if (LightFlowOverlayLeftDownIn == null) CustomLogger.PrintErr((nameof(LightFlowOverlayLeftDownIn)).ToString());
            if (LightFlowOverlayRightUpIn == null) CustomLogger.PrintErr((nameof(LightFlowOverlayRightUpIn)).ToString());
            if (LightFlowOverlayRightDownIn == null) CustomLogger.PrintErr((nameof(LightFlowOverlayRightDownIn)).ToString());
            
			copy.LightFlowOverlayLeftUpIn = LightFlowOverlayLeftUpIn.Duplicate() as AnimatedSprite2D;
			copy.LightFlowOverlayLeftDownIn = LightFlowOverlayLeftDownIn.Duplicate() as AnimatedSprite2D;
			copy.LightFlowOverlayRightUpIn = LightFlowOverlayRightUpIn.Duplicate() as AnimatedSprite2D;
			copy.LightFlowOverlayRightDownIn = LightFlowOverlayRightDownIn.Duplicate() as AnimatedSprite2D;
			return copy;
		}

	}
}
