using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : TextureRect
    {
        [Export] public int WidthInTiles { get; private set; }
        [Export] public int HeightInTiles { get; private set; }
        
        public GridViewModel ViewModel { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        protected List<AnimationSlot> AnimationSlots = new();

        public void Initialize(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, GridViewModel viewModel)
        {
            if (WidthInTiles == 0) CustomLogger.PrintErr($"{nameof(WidthInTiles)} cannot be 0");
            if (HeightInTiles == 0) CustomLogger.PrintErr($"{nameof(HeightInTiles)} cannot be 0");
            this.GridX = gridX;
            this.GridY = gridY;
            this.ViewModel = viewModel;
            this.RotationDegrees = rotationCounterClockwise.ToDegreesClockwise();
            var rawposition = new Vector2(this.GridX * GameManager.TilePixelSize, this.GridY * GameManager.TilePixelSize);
            Position = rawposition + GetPositionDisplacementAfterRotation();
            Visible = true;
        }

        public bool IsPlacedOnGrid()
        {
            if (ViewModel == null) return false;
            if (ViewModel.IsInGrid(GridX, GridY, WidthInTiles, HeightInTiles) == false) return false;
            if (ViewModel.GridComponentViews[this.GridX, this.GridY] != this) return false;
            return true;
        }
        public Vector2 GetPositionDisplacementAfterRotation()
        {
            var tilePixelSize = GameManager.TilePixelSize;
            var borderLeftDown = GameManager.TileBorderLeftDown;
            var displacement = new Vector2();
            int roundedRotation = (int)Math.Round(this.RotationDegrees / 90) * 90;
            switch (roundedRotation)
            {
                case 270:
                    displacement = new Vector2(0, HeightInTiles * tilePixelSize- borderLeftDown);
                    break;
                case 90: // Assuming you have a corresponding enumeration value
                    displacement = new Vector2(WidthInTiles * tilePixelSize - borderLeftDown, 0);
                    break;
                case 180:
                    displacement = new Vector2(WidthInTiles * tilePixelSize - borderLeftDown, HeightInTiles * tilePixelSize - borderLeftDown);
                    break;
            }
            return displacement;
        }
        protected static AnimatedSprite2D CreateAnimation(AnimatedSprite2D baseAnimation )
        {
            var anim = baseAnimation.Duplicate() as AnimatedSprite2D;
            anim.Autoplay = baseAnimation.SpriteFrames.GetAnimationNames().First();
            baseAnimation.GetParent().AddChild(anim);
            anim.Hide();
            return anim;
        }
        public void HideLightVector()
        {
            foreach (var slot in AnimationSlots)
            {
                slot.OverlayInFlow.Autoplay = "";
                slot.OverlayOutFlow.Autoplay = "";
                slot.OverlayInFlow.Stop();
                slot.OverlayOutFlow.Stop();
                slot.OverlayInFlow.Hide();
                slot.OverlayOutFlow.Hide();
            }
        }
        public void DisplayLightVector(List<LightAtPin> lightsAtPins)
        {
            try
            {
                foreach (LightAtPin light in lightsAtPins)
                {
                    TryStartAnimationOverlay(light);
                    TryStartAnimationOverlay(light, true);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.PrintErr(ex.Message);
            }

        }

        protected bool TryStartAnimationOverlay(LightAtPin lightAtPin, bool isOutFlow = false)
        {
            var animationSlot = AnimationSlot.TryFindMatching(AnimationSlots, lightAtPin );
            if (animationSlot == null) return false;
            var overlay = animationSlot.OverlayInFlow;
            overlay.Play();
            float alpha = (float)lightAtPin.lightInFlow.Real;

            if (isOutFlow)
            {
                overlay = animationSlot.OverlayOutFlow;
                alpha = (float)lightAtPin.lightOutFlow.Real;
                overlay.PlayBackwards();
            }
            CustomLogger.PrintLn(lightAtPin.ToString());

            overlay.Show();
            overlay.Modulate = new Godot.Color(lightAtPin.color.ToGodotColor(), alpha);
            return true;
        }
        public override void _GuiInput(InputEvent inputEvent)
        {
            base._GuiInput(inputEvent);
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                if ( mouseEvent.Position.X < 0
                    || mouseEvent.Position.Y < 0
                    || mouseEvent.Position.X > this.WidthInTiles* GameManager.TilePixelSize
                    || mouseEvent.Position.Y > this.HeightInTiles * GameManager.TilePixelSize
                    || ViewModel == null)
                {
                    return;
                }
                if (mouseEvent.ButtonIndex == MouseButton.Middle && mouseEvent.Pressed)
                {
                    ViewModel.DeleteComponentCommand.Execute(new DeleteComponentArgs(GridX, GridY));
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    var args = new RotateComponentArgs(GridX, GridY);
                    if (ViewModel.RotateComponentCommand.CanExecute(args))
                    {
                        ViewModel.RotateComponentCommand.Execute(args);
                    }
                    else
                    {
                        // Error Animation
                    }

                }
            }
        }
        public virtual ComponentBaseView Duplicate()
        {
            var copy = base.Duplicate() as ComponentBaseView;
            return copy;
        }

		protected static List<AnimationSlot> CreateTriColorAnimSlot(int offsetX, int offsetY, RectSide side, AnimatedSprite2D animDraft)
		{
			List<AnimationSlot> animSlots = new();
            animSlots.Add(new AnimationSlot(LightColor.Red, offsetX, offsetY, side, CreateAnimation(animDraft), CreateAnimation(animDraft)));
			animSlots.Add(new AnimationSlot(LightColor.Green, offsetX, offsetY, side, CreateAnimation(animDraft), CreateAnimation(animDraft)));
            animSlots.Add(new AnimationSlot(LightColor.Blue, offsetX, offsetY, side, CreateAnimation(animDraft), CreateAnimation(animDraft)));
			return animSlots;
        }
    }
}
