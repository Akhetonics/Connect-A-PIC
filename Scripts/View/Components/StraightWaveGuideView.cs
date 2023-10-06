using CAP_Core.ExternalPorts;
using ConnectAPIC.Scripts.Debuggers;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class StraightWaveGuideView : ComponentBaseView
	{

		[Export] protected AnimatedSprite2D LightOverlay;
		[Export] protected PinView LeftPin;
		[Export] protected PinView RightPin;
		private List<AnimationSlot> AnimationSlots;
		public StraightWaveGuideView()
		{
		}

		public override void DisplayLightVector(List<LightAtPin> lightsAtPins)
		{
			LightOverlay.Show();
			LightOverlay.Play();
            TreePrinter.PrintTree(LightOverlay.GetParent(), 0);
            //try
            //{
            //	var left = lightsAtPins.Single(l => l.side == CAP_Core.Tiles.RectSide.Left);
            //	StartAnimationOverlay(left);
            //} catch (Exception ex)
            //{
            //	CustomLogger.PrintErr(ex.Message); 
            //	throw;
            //}

        }

		private void StartAnimationOverlay(LightAtPin lightAtPin, bool isOutFlow = false)
		{
			var animationSlot = AnimationSlot.FindMatching(AnimationSlots, lightAtPin, true);
			var overlay = animationSlot.OverlayInFlow;
            TreePrinter.PrintTree(overlay.GetParent(), 0);
            overlay.Play("default");
            float alpha = (float)lightAtPin.lightInFlow.Real;
            
            if (isOutFlow)
			{
				overlay = animationSlot.OverlayOutFlow;
				alpha = (float)lightAtPin.lightOutFlow.Real;
                overlay.PlayBackwards("default");
            }
			CustomLogger.PrintLn(lightAtPin.ToString());

            overlay.Show();
            overlay.Modulate = new Color(lightAtPin.color.ToGodotColor(), alpha);
		}

		public override void HideLightVector()
		{
			foreach(var slot in AnimationSlots)
			{
				slot.OverlayInFlow.Autoplay = "";
				slot.OverlayOutFlow.Autoplay = "";
				slot.OverlayInFlow.Stop();
				slot.OverlayOutFlow.Stop();
				slot.OverlayInFlow.Hide();
				slot.OverlayOutFlow.Hide();
			}
		}

		private AnimatedSprite2D CreateAnimation(AnimatedSprite2D baseAnimation)
		{
			var anim = baseAnimation.Duplicate() as AnimatedSprite2D;
			anim.Autoplay = baseAnimation.SpriteFrames.GetAnimationNames().First();
			LightOverlay.GetParent().AddChild(anim);
			var parentsChildren = LightOverlay.GetParent().GetChildren().Cast<AnimatedSprite2D>();
			anim.Hide();
			return anim;
		}
		public override void _Ready()
		{
			base._Ready();
			//if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is not assigned");

			AnimationSlots = new List<AnimationSlot>();
			//AnimationSlots = new List<AnimationSlot>()
			//{
			//	new AnimationSlot(LightColor.Red, 0, 0, default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay)),
			//	new AnimationSlot(LightColor.Green, 0, 0,default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay)),
			//	new AnimationSlot(LightColor.Blue, 0, 0,default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay))
			//};

		}

	}
}
