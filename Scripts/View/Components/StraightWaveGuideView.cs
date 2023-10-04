using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public class AnimationSlot
	{
		public AnimationSlot(LightColor color, int offsetX, int offsetY, RectSide side, AnimatedSprite2D overlayIn , AnimatedSprite2D overlayOut) {
			this.Color = color;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
			Side = side;
			this.OverlayInFlow = overlayIn;
			this.OverlayOutFlow = overlayOut;
		}
		public bool IsMatchingWithLightVector(LightAtPin lightVector, bool ignoreSide = false)
		{
			if (OffsetX != lightVector.partOffsetX) return false;
			if (OffsetY != lightVector.partOffsetY) return false;
			if (ignoreSide == false && Side != lightVector.side) return false;
			if (Color != lightVector.color) return false;
			return true;
		}
		public static AnimationSlot FindMatching(List<AnimationSlot> slots, LightAtPin lightVector, bool ignoreSide = false)
		{
			try
			{
				return slots.Single(s => s.IsMatchingWithLightVector(lightVector, ignoreSide));
			}
			catch (Exception ex)
			{
				CustomLogger.PrintErr(ex.Message);
				throw;
			}
		}
		public LightColor Color { get; }
		public int OffsetX { get; }
		public int OffsetY { get; }
		public RectSide Side { get; }
		public AnimatedSprite2D OverlayInFlow { get; }
		public AnimatedSprite2D OverlayOutFlow { get; }
	}
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
			try
			{
				var left = lightsAtPins.Single(l => l.side == CAP_Core.Tiles.RectSide.Left);
				StartAnimationOverlay(left);
			} catch (Exception ex)
			{
				CustomLogger.PrintErr(ex.Message); 
				throw;
			}
			
		}

		private void StartAnimationOverlay(LightAtPin lightAtPin, bool isOutFlow = false)
		{
			var animationSlot = AnimationSlot.FindMatching(AnimationSlots, lightAtPin, true);
			var overlay = animationSlot.OverlayInFlow;
			float alpha = (float)lightAtPin.lightInFlow.Real;
			if (isOutFlow)
			{
				overlay = animationSlot.OverlayOutFlow;
				alpha = (float)lightAtPin.lightOutFlow.Real;
			}
			alpha = 1;
			overlay.Modulate = new Color(lightAtPin.color.ToGodotColor(), alpha);
            overlay.Autoplay = "default";
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
			var anim = new AnimatedSprite2D();
			anim.SpriteFrames = baseAnimation.SpriteFrames;
			anim.Position = new Vector2(32,32);
			//anim.Autoplay = baseAnimation.SpriteFrames.GetAnimationNames().First();
			LightOverlay.GetParent().AddChild(anim);
			
            return anim;
		}
		public override void _Ready()
		{
			base._Ready();
			if (LightOverlay == null) CustomLogger.PrintErr(nameof(LightOverlay) + " is not assigned");
			
			AnimationSlots = new List<AnimationSlot>()
			{
				new AnimationSlot(LightColor.Red, 0, 0, default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay)),
				new AnimationSlot(LightColor.Green, 0, 0,default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay)),
				new AnimationSlot(LightColor.Blue, 0, 0,default, CreateAnimation(LightOverlay), CreateAnimation(LightOverlay))
			};

			//AnimationSlots.First().OverlayInFlow.Show();
			//AnimationSlots.First().OverlayInFlow.Play();
		}

	}
}
