﻿using CAP_Core.Component.ComponentHelpers;
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
using System.Runtime.CompilerServices;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : TextureRect
    {
        [Export] public int WidthInTiles { get; private set; }
        [Export] public int HeightInTiles { get; private set; }

        public GridViewModel ViewModel { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        protected List<AnimationSlot> AnimationSlots;
        private new float RotationDegrees { get => base.RotationDegrees; set => base.RotationDegrees = value; }
        private new float Rotation { get => base.Rotation; set => base.Rotation = value; }
        private DiscreteRotation _rotationCC;
        public DiscreteRotation RotationCC { 
            get => _rotationCC; 
            set { 
                _rotationCC = value;
                AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(value));
                RotationDegrees = value.ToDegreesClockwise(); 
            } 
        }
        
        public override void _Ready()
        {
            base._Ready();
            CallDeferred(nameof(RefreshRotation));
        }
        public void RefreshRotation()
        {
            RotationCC = RotationCC;
        }
        public void Initialize(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, GridViewModel viewModel)
        {
            if(AnimationSlots == null)
            {
                _Ready();
            }
            if (WidthInTiles == 0) CustomLogger.PrintErr($"{nameof(WidthInTiles)} cannot be 0");
            if (HeightInTiles == 0) CustomLogger.PrintErr($"{nameof(HeightInTiles)} cannot be 0");
            this.GridX = gridX;
            this.GridY = gridY;
            this.ViewModel = viewModel;
            this.RotationCC = rotationCounterClockwise;
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
            int roundedRotation = (int)Math.Round(RotationDegrees / 90) * 90;
            CustomLogger.PrintLn("rotation: " + roundedRotation + " Rotdisc " + RotationCC );
            switch (roundedRotation)
            {
                case 270:
                    displacement = new Vector2(0, HeightInTiles * tilePixelSize - borderLeftDown);
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
        protected static AnimatedSprite2D CreateAnimation(AnimatedSprite2D baseAnimation)
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
                slot.OverlayInFlow.Modulate = new Godot.Color(0, 0, 0, 0);
                slot.OverlayOutFlow.Modulate = new Godot.Color(0, 0, 0, 0);
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
                    StartAnimationForLight(light);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.PrintErr(ex.Message);
            }

        }

        private void StartAnimationForLight(LightAtPin light)
        {
            var animationSlot = AnimationSlot.TryFindMatching(AnimationSlots, light);
            if (animationSlot == null) return;
            PlayOverlayAnimation(light, animationSlot.OverlayInFlow , (float)light.lightInFlow.Real  ,  1);
            PlayOverlayAnimation(light, animationSlot.OverlayOutFlow, (float)light.lightOutFlow.Real , -1);
        }

        protected void PlayOverlayAnimation(LightAtPin lightAtPin, AnimatedSprite2D overlay, float alpha, float playspeed)
        {
            overlay.Play(null, playspeed);
            if(playspeed == -1)
            {
                CustomLogger.PrintLn(lightAtPin.ToString());
            }
            
            overlay.Show();
            overlay.Modulate += new Godot.Color(lightAtPin.color.ToGodotColor(), alpha);
        }
        public override void _GuiInput(InputEvent inputEvent)
        {
            base._GuiInput(inputEvent);
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.Position.X < 0
                    || mouseEvent.Position.Y < 0
                    || mouseEvent.Position.X > this.WidthInTiles * GameManager.TilePixelSize
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
            copy.RotationCC = this.RotationCC;
            return copy;
        }

        protected List<AnimationSlot> CreateTriColorAnimSlot(int offsetx, int offsety, RectSide side, AnimatedSprite2D animDraft)
        {
            Vector2I sizeInTiles = new Vector2I(WidthInTiles, HeightInTiles);
            Vector2I offset = new Vector2I(offsetx, offsety);
            List<AnimationSlot> animSlots = new()
            {
                new AnimationSlot(LightColor.Red, offset, side, CreateAnimation(animDraft), CreateAnimation(animDraft) , sizeInTiles),
                new AnimationSlot(LightColor.Green, offset, side, CreateAnimation(animDraft), CreateAnimation(animDraft), sizeInTiles),
                new AnimationSlot(LightColor.Blue, offset, side, CreateAnimation(animDraft), CreateAnimation(animDraft), sizeInTiles)
            };
            return animSlots;
        }
    }
}
