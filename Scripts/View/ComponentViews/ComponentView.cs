﻿using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentViews;
using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class ComponentView : TextureRect
    {
        public delegate void SliderChangedEventHandler(ComponentView view, Godot.Slider slider, double newVal);
        public event SliderChangedEventHandler SliderChanged;
        public const string SliderNumberMetaID = "SliderNumber";
        public int WidthInTiles { get; private set; }
        public int HeightInTiles { get; private set; }
        public ILogger Logger { get; private set; }
        public int TypeNumber { get; set; }
        private Sprite2D OverlayBluePrint { get; set; }
        public Sprite2D OverlayRed { get; private set; } // each laser(color) is independent of the others, so they need their own overlay and shader
        public Sprite2D OverlayGreen { get; private set; }
        public Sprite2D OverlayBlue { get; private set; }
        private List<Sprite2D> OverlaySprites { get; set; } = new();
        private List<Godot.Slider> Sliders { get; set; } = new();
        public GridViewModel ViewModel { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        public List<AnimationSlot> AnimationSlots { get; private set; } = new();
        private new float RotationDegrees { get => base.RotationDegrees; set => base.RotationDegrees = value; }
        private new float Rotation { get => base.Rotation; set => base.Rotation = value; }
        private DiscreteRotation _rotationCC;
        public ShaderMaterial LightOverlayShader { get; set; }
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
            RotationCC = _rotationCC;
        }

        private void FindAndAssignOverlayBlueprint()
        {
            OverlayBluePrint = FindChild("Overlay", true, false) as Sprite2D;

            if (OverlayBluePrint == null)
            {
                OverlayBluePrint = FindChild("?verlay", true, false) as Sprite2D;
            }
            if (OverlayBluePrint == null)
            {
                OverlayBluePrint = FindChild("?verlay*", true, false) as Sprite2D;
            }
        }

        // initialize one of the existing sliders
        public void InitializeSlider(int sliderNumber, string sliderName, string sliderLabelName, double minVal, double maxVal, double steps)
        {
            var label = FindChild(sliderLabelName, true, false) as RichTextLabel;
            var godotSlider = FindChild(sliderName, true, false) as Godot.Slider;
            godotSlider.MinValue = minVal;
            godotSlider.MaxValue = maxVal;
            godotSlider.SetMeta(SliderNumberMetaID, sliderNumber);
            godotSlider.Value = (minVal + maxVal) / 2;
            godotSlider.Step = (maxVal - minVal) / steps; // step is the distance between two steps in value
            godotSlider.ValueChanged += (newVal) =>
            {
                label.Text = $"[center]{newVal:F2}";
                SliderChanged?.Invoke(this, godotSlider, newVal);
            };
            this.Sliders.Add(godotSlider);
        }

        public void InitializeComponent(int componentTypeNumber, List<SliderViewData> sliderDataSets, List<AnimationSlotOverlayData> slotDataSets, int widthInTiles, int heightInTiles, ILogger logger)
        {
            this.Logger = logger;
            if (widthInTiles == 0) Logger.PrintErr(nameof(widthInTiles) + " of this element is not set in the TypeNR: " + componentTypeNumber);
            if (heightInTiles == 0) Logger.PrintErr(nameof(heightInTiles) + " of this element is not set in the TypeNR: " + componentTypeNumber);
            InitializeSliders(sliderDataSets);
            this.TypeNumber = componentTypeNumber;
            this.WidthInTiles = widthInTiles;
            this.HeightInTiles = heightInTiles;
            LightOverlayShader = new ShaderMaterial();
            LightOverlayShader.Shader = ResourceLoader.Singleton.Load("res://Scenes/Components/LightOverlayShaded.tres").Duplicate() as Shader;
            FindAndAssignOverlayBlueprint();
            this.CheckForNull(x => x.OverlayBluePrint);
            InitializeLightOverlays();
            foreach (var slotData in slotDataSets)
            {
                if (slotData.LightFlowOverlay == null)
                {
                    Logger.PrintErr(nameof(slotData.LightFlowOverlay) + " is null in TypeNR: " + componentTypeNumber);
                }
                AnimationSlots.AddRange(CreateRGBAnimSlots(slotData.Side, slotData.LightFlowOverlay, slotData.OffsetX, slotData.OffsetY));
            }
            RotationCC = _rotationCC;
        }
        private void InitializeSliders(List<SliderViewData> newSliders)
        {

            foreach (var slider in newSliders)
            {
                
            }
        }

        private void InitializeLightOverlays()
        {
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
            var rawPosition = new Vector2(this.GridX * GameManager.TilePixelSize, this.GridY * GameManager.TilePixelSize);
            Position = rawPosition + GetPositionDisplacementAfterRotation();
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
            newOverlay.Material = LightOverlayShader.Duplicate() as ShaderMaterial;
            (newOverlay.Material as ShaderMaterial).SetShaderParameter("laserColor", laserColor);
            return newOverlay;
        }
        public void HideLightVector()
        {
            OverlayRed?.Hide();
            OverlayGreen?.Hide();
            OverlayBlue?.Hide();
            ResetAllShaderParametersToZero();
        }

        private void ResetAllShaderParametersToZero()
        {
            var emptyTexture = new Texture();
            foreach (AnimationSlot slot in AnimationSlots)
            {
                if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
                {
                    for (int i = 0; i < AnimationSlot.MaxShaderAnimationSlots; i++) // all parameters in the whole shader should be set to zero
                    {
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightInFlow + i, Vector4.Zero);
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightOutFlow + i, Vector4.Zero);
                        shaderMat.SetShaderParameter(ShaderParameterNames.Animation + i, emptyTexture);
                        shaderMat.SetShaderParameter(ShaderParameterNames.LightColor, new Godot.Color(0, 0, 0));
                    }
                }
            }
        }

        public virtual void DisplayLightVector(List<LightAtPin> lightsAtPins)
        {
            int shaderAnimNumber = 1;
            foreach (LightAtPin light in lightsAtPins)
            {
                var matchingSlots = AnimationSlot.FindMatching(AnimationSlots, light);
                matchingSlots.ForEach(slot =>
                {
                    AssignInAndOutFlowShaderData(slot, light, shaderAnimNumber);
                    shaderAnimNumber++;
                });
            }
            OverlayRed?.Show();
            OverlayGreen?.Show();
            OverlayBlue?.Show();
        }
        protected void AssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderSlotNumber)
        {
            if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
            {
                var InFlowDataAndPosition = new Godot.Vector4((float)lightAtPin.lightInFlow.Magnitude, (float)lightAtPin.lightInFlow.Phase, 0, 0);
                var outFlowDataAndPosition = new Vector4((float)lightAtPin.lightOutFlow.Magnitude, (float)lightAtPin.lightOutFlow.Phase, 0, 0);
                shaderMat.SetShaderParameter(ShaderParameterNames.LightInFlow + shaderSlotNumber, InFlowDataAndPosition);
                shaderMat.SetShaderParameter(ShaderParameterNames.LightOutFlow + shaderSlotNumber, outFlowDataAndPosition);
                shaderMat.SetShaderParameter(ShaderParameterNames.Animation + shaderSlotNumber, slot.Texture);
                shaderMat.SetShaderParameter(ShaderParameterNames.LightColor, slot.MatchingLaser.Color.ToGodotColor());
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
                    ViewModel.DeleteComponentCommand.ExecuteAsync(new DeleteComponentArgs(GridX, GridY)).RunSynchronously();
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    var args = new RotateComponentArgs(GridX, GridY);
                    if (ViewModel.RotateComponentCommand.CanExecute(args))
                    {
                        ViewModel.RotateComponentCommand.ExecuteAsync(args).RunSynchronously();
                    }
                    else
                    {
                        // Error Animation
                    }

                }
            }
        }

        public virtual ComponentView Duplicate()
        {
            var copy = base.Duplicate() as ComponentView;
            copy.RotationCC = this.RotationCC; // give the new copy the proper RotationCC so that it has the correct rotation
            return copy;
        }

        protected List<AnimationSlot> CreateRGBAnimSlots(RectSide inflowSide, Texture overlayAnimTexture, int tileOffsetX = 0, int tileOffsetY = 0)
        {
            var tileOffset = new Vector2I(tileOffsetX, tileOffsetY);
            return new List<AnimationSlot>()
            {
                new (LaserType.Red, tileOffset, inflowSide, OverlayRed, overlayAnimTexture,new Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Green,tileOffset, inflowSide, OverlayGreen, overlayAnimTexture, new Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Blue,tileOffset, inflowSide, OverlayBlue, overlayAnimTexture, new Vector2I(WidthInTiles, HeightInTiles)),
            };
        }


    }
}
