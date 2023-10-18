using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.Debuggers;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class StraightWaveGuideView : ComponentBaseView
	{

		[Export] protected AnimatedSprite2D LightOverlay;
		[Export] protected Label leftComlex;
		[Export] protected Label rightComplex;

		public override void InitializeAnimationSlots()
		{
			if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is not assigned");
			AnimationSlots = CreateTriColorAnimSlot(0, 0, RectSide.Left, LightOverlay);
		}
		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			base.DisplayLightVector(lightsAtPins);
			if (lightsAtPins.First().color != LightColor.Red) return;
			var leftLight = lightsAtPins.First(l => l.side == RectSide.Left);
			var leftIn = leftLight.lightInFlow;
			var leftOut = leftLight.lightOutFlow;
			var leftPhase = (leftIn.Phase * (180.0 / Math.PI));
			var combinedlight = leftIn + leftOut;
			var combinedPhase = (leftIn.Phase * (180.0 / Math.PI));
			
			rightComplex.Text = $"{(combinedPhase):F0}\nM {(combinedlight).Magnitude:F1}";
		}

	}
}
