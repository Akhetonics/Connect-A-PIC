using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
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

		[Export] protected Texture OverlayAnimTexture;
		[Export] protected Label leftComplex;
		[Export] protected Label rightComplex;

		public override void InitializeAnimationSlots()
		{
			this.CheckForNull(x => x.OverlayAnimTexture);
			this.CheckForNull(x => x.leftComplex);
			this.CheckForNull(x => x.rightComplex);
			AnimationSlots.AddRange(CreateRGBAnimSlots(RectSide.Left, OverlayAnimTexture));
		}
		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			base.DisplayLightVector(lightsAtPins);
			if (lightsAtPins.First().color != LightColor.Red) return;
			var leftLight = lightsAtPins.First(l => l.side == RectSide.Left.RotateSideClockwise(this.RotationCC));
			var leftIn = leftLight.lightInFlow;
			var leftOut = leftLight.lightOutFlow;
			var leftPhase = (leftIn.Phase * (180.0 / Math.PI));
			var rightPhase = (leftOut.Phase * (180.0 / Math.PI));
			
			leftComplex.Text = $"{(leftPhase):F0}\nM {(leftIn).Magnitude:F1}";
			rightComplex.Text = $"{(rightPhase):F0}\nM {(leftOut).Magnitude:F1}";
		}

	}
}
