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
        [Export] private Sprite2D OverlayBluePrint { get; set; }
        public Sprite2D OverlayRed { get; private set; }
        public Sprite2D OverlayGreen { get; private set; }
        public Sprite2D OverlayBlue { get; private set; }
        private List<Sprite2D> OverlaySprites { get; set; } = new();
        public GridViewModel ViewModel { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        protected List<AnimationSlot> AnimationSlots = new();
        private new float RotationDegrees { get => base.RotationDegrees; set => base.RotationDegrees = value; }
        private new float Rotation { get => base.Rotation; set => base.Rotation = value; }
        public abstract void InitializeAnimationSlots();
        private DiscreteRotation _rotationCC;
        public DiscreteRotation RotationCC
        {
            get => _rotationCC;
            set
            {
                _rotationCC = value;
                AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(value));
                RotationDegrees = value.ToDegreesClockwise();
            }
        }

        public override void _Ready()
        {
            base._Ready();
            if (WidthInTiles == 0) CustomLogger.PrintErr(nameof(WidthInTiles) + " of this element is not set in the ComponentScene: " + this.GetType().Name);
            if (HeightInTiles == 0) CustomLogger.PrintErr(nameof(HeightInTiles) + " of this element is not set in the ComponentScene: " + this.GetType().Name);
            InitializeLightOverlays();
            InitializeAnimationSlots();
            RotationCC = _rotationCC;
        }

        private void InitializeLightOverlays()
        {
            this.CheckForNull(x => x.OverlayBluePrint);
            if (OverlayBluePrint == null) return;
            OverlayBluePrint.Hide();
            OverlayRed = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Red.ToGodotColor());
            OverlayGreen = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Green.ToGodotColor());
            OverlayBlue = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Blue.ToGodotColor());

            OverlaySprites.Add(OverlayRed);
            OverlaySprites.Add(OverlayGreen);
            OverlaySprites.Add(OverlayBlue);
        }

        public void RegisterInGrid(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, GridViewModel viewModel)
        {
            this.GridX = gridX;
            this.GridY = gridY;
            this.ViewModel = viewModel;
            this.RotationCC = rotationCounterClockwise;
            var rawposition = new Vector2(this.GridX * GameManager.TilePixelSize, this.GridY * GameManager.TilePixelSize);
            Position = rawposition + GetPositionDisplacementAfterRotation();
            Visible = true;
            HideLightVector();
        }

        public bool IsPlacedOnGrid()
        {
            if (ViewModel == null) return false;
            if (!ViewModel.IsInGrid(GridX, GridY, WidthInTiles, HeightInTiles)) return false;
            if (ViewModel.GridComponentViews[this.GridX, this.GridY] != this) return false;
            return true;
        }
        public Vector2 GetPositionDisplacementAfterRotation()
        {
            var tilePixelSize = GameManager.TilePixelSize;
            var borderLeftDown = GameManager.TileBorderLeftDown;
            var displacement = new Vector2();
            int roundedRotation = (int)Math.Round(RotationDegrees / 90) * 90;
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
        protected Sprite2D DuplicateAndConfigureOverlay(Sprite2D overlayDraft, Godot.Color laserColor)
        {
            var newOverlay = overlayDraft.Duplicate() as Sprite2D;
            overlayDraft.GetParent().AddChild(newOverlay);
            newOverlay.Hide();
            if (overlayDraft.Material is ShaderMaterial materialDraft)
            {
                newOverlay.Material = materialDraft.Duplicate(true) as ShaderMaterial;
                (newOverlay.Material as ShaderMaterial).SetShaderParameter("laserColor", laserColor);
            }
            return newOverlay;
        }
        public void HideLightVector()
        {
            OverlayRed?.Hide();
            OverlayGreen?.Hide();
            OverlayBlue?.Hide();
        }
        public virtual void DisplayLightVector(List<LightAtPin> lightsAtPins)
        {
            try
            {
                int shaderAnimNumber = 1;
                foreach (LightAtPin light in lightsAtPins)
                {
                    var animationSlot = AnimationSlot.TryFindMatching(AnimationSlots, light);
                    if (animationSlot == null) continue;
                    AssignInAndOutFlowShaderData(animationSlot, light, shaderAnimNumber);
                    shaderAnimNumber += 1;
                }
                OverlayRed?.Show();
                OverlayGreen?.Show();
                OverlayBlue?.Show();
            }
            catch (Exception ex)
            {
                CustomLogger.PrintErr(ex.Message);
            }
        }
        protected void AssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderAnimationNumber)
        {
            if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
            {
                var InFlowDataAndPosition = new Godot.Vector4((float)lightAtPin.lightInFlow.Magnitude, (float)lightAtPin.lightInFlow.Phase,0,0);
                var outFlowDataAndPosition = new Vector4((float)lightAtPin.lightOutFlow.Magnitude, (float)lightAtPin.lightOutFlow.Phase,0,0);
                shaderMat.SetShaderParameter("lightInFlow" + shaderAnimationNumber, InFlowDataAndPosition);
                shaderMat.SetShaderParameter("lightOutFlow" + shaderAnimationNumber, outFlowDataAndPosition);
                shaderMat.SetShaderParameter("animation" + shaderAnimationNumber, slot.Texture);
                shaderMat.SetShaderParameter("lightColor", slot.Color.ToGodotColor());
            }
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

        protected List<AnimationSlot> CreateRGBAnimSlots(RectSide inflowSide, Texture overlayAnimTexture, int tileOffsetX = 0, int tileOffsetY = 0)
        {
            var tileOffset = new Vector2I(tileOffsetX, tileOffsetY);
            return new List<AnimationSlot>()
            {
                new AnimationSlot(LightColor.Red, tileOffset, inflowSide, OverlayRed, overlayAnimTexture,new Vector2I(WidthInTiles, HeightInTiles)),
                new AnimationSlot(LightColor.Green,tileOffset, inflowSide, OverlayGreen, overlayAnimTexture, new Vector2I(WidthInTiles, HeightInTiles)),
                new AnimationSlot(LightColor.Blue,tileOffset, inflowSide, OverlayBlue, overlayAnimTexture, new Vector2I(WidthInTiles, HeightInTiles)),
            };
        }


    }
}
