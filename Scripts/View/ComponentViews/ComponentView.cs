using CAP_Contracts.Logger;
using CAP_Core;
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
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using Godot.Collections;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using YamlDotNet.Core.Tokens;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class ComponentView : TextureRect 
    {
        public int WidthInTiles { get; set; }
        public int HeightInTiles { get; set; }
        private List<Godot.Slider> Sliders { get; set; } = new();
        private Node2D RotationArea { get; set; } // the part of the component that rotates
        public Sprite2D OverlayBluePrint { get; set; }
        public Sprite2D OverlayRed { get; private set; } // each laser(color) is independent of the others, so they need their own overlay and shader
        public Sprite2D OverlayGreen { get; private set; }
        public Sprite2D OverlayBlue { get; private set; }
        public List<AnimationSlot> AnimationSlots { get; private set; } = new();
        private List<AnimationSlotOverlayData> AnimationSlotRawData { get; set; } = new();
        private List<Sprite2D> OverlaySprites { get; set; } = new();
        public ShaderMaterial LightOverlayShader { get; set; }
        public ComponentViewModel ViewModel { get; private set; }
        public const string SliderNumberMetaID = "SliderNumber";
        public const string SliderLabelMetaID = "SliderLabel";
        public const string SliderIsCallbackRegistered = "SliderIsCallbackRegistered";
        public new float RotationDegrees{
            get => RotationArea?.RotationDegrees ?? 0;
            set{
                if (RotationArea?.RotationDegrees != null)
                    RotationArea.RotationDegrees = value;
            }
        }
        private new float Rotation { get => RotationArea.Rotation; set => RotationArea.Rotation = value; }

        public ComponentView()
        {
            ViewModel = new ComponentViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.SliderData.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                if(e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach ( var slider in e.NewItems.Cast<SliderViewData>().ToList())
                    {
                        FindAndInitializeSlider(slider);
                    }
                }
            };
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComponentViewModel.RotationCC))
            {
                AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(ViewModel.RotationCC)); // the slots need to know the rotation for proper animation matching
                RotationDegrees = ViewModel.RotationCC.ToDegreesClockwise();
            } 
            else if (e.PropertyName == nameof(ComponentViewModel.LightsAtPins))
            {
                var lightAndSlots = new List<(LightAtPin light, List<AnimationSlot>)>();
                var shaderSlotNumber = 1;
                foreach (LightAtPin light in ViewModel.LightsAtPins)
                {
                    var slots = AnimationSlot.FindMatching(AnimationSlots, light);
                    foreach(var slot in slots)
                    {
                        this.ShowAndAssignInAndOutFlowShaderData(slot, light, shaderSlotNumber);
                        shaderSlotNumber++;
                    }
                }
            }
            else if (e.PropertyName == nameof(ComponentViewModel.SliderData))
            {
                Console.WriteLine();
            }
            else if (e.PropertyName == nameof(ComponentViewModel.Position))
            {
                Position = new Godot.Vector2( ViewModel.Position.X , ViewModel.Position.Y);
            }
            else if (e.PropertyName == nameof(ComponentViewModel.Visible))
            {
                Visible = ViewModel.Visible;
            }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialize(List<AnimationSlotOverlayData> animationSlotOverlays, int widthInTiles, int heightInTiles)
        {
            RotationArea = (FindChild("?otation*", true, false) ?? FindChild("ROTATION*", true, false)) as Node2D;
            this.WidthInTiles = widthInTiles;
            this.HeightInTiles = heightInTiles;
            LightOverlayShader = new ShaderMaterial();
            LightOverlayShader.Shader = ResourceLoader.Load("res://Scenes/Components/LightOverlayShaded.tres").Duplicate() as Shader;
            AnimationSlotRawData = animationSlotOverlays;
            FindAndCreateLightOverlays(); // first create the overlays, then use them to create the animationSlots
            AnimationSlots = ConvertSlotOverlayDataToAnimationSlots(animationSlotOverlays);
        }

        public List<AnimationSlot> ConvertSlotOverlayDataToAnimationSlots(List<AnimationSlotOverlayData> animationSlotOverlays)
        {
            List<AnimationSlot> slots = new();
            foreach (var slotData in animationSlotOverlays)
            {
                var lightOverlayBaseTexture = ResourceLoader.Load<Texture2D>(slotData.LightFlowOverlayPath);
                slots.AddRange(CreateRGBAnimSlots(slotData.Side, lightOverlayBaseTexture, new Vector2I(slotData.OffsetX, slotData.OffsetY)));
            }
            return slots;
        }

        private void FindOverlayBlueprint()
        {
            OverlayBluePrint = FindChild("Overlay", true, false) as Sprite2D;
            OverlayBluePrint ??= FindChild("?verlay", true, false) as Sprite2D;
            OverlayBluePrint ??= FindChild("*?verlay*", true, false) as Sprite2D;
        }
        protected List<AnimationSlot> CreateRGBAnimSlots(RectSide inflowSide, Godot.Texture overlayAnimTexture, Vector2I tileOffset)
        {
            return new List<AnimationSlot>()
            {
                new (LaserType.Red, tileOffset, inflowSide, OverlayRed, overlayAnimTexture,new Godot.Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Green,tileOffset, inflowSide, OverlayGreen, overlayAnimTexture, new Godot.Vector2I(WidthInTiles, HeightInTiles)),
                new (LaserType.Blue,tileOffset, inflowSide, OverlayBlue, overlayAnimTexture, new Godot.Vector2I(WidthInTiles, HeightInTiles)),
            };
        }
        private void SetSliderValue(int sliderNumber, double value)
        {
            var slider = Sliders.Single(s => (int)s.GetMeta(SliderNumberMetaID) == sliderNumber);
            var label = (RichTextLabel)slider.GetMeta(SliderLabelMetaID);
            slider.Value = value;
            SetSliderLabelText(label, value);
        }

        // initialize one of the existing sliders
        private void FindAndInitializeSlider(SliderViewData sliderData)
        {
            var label = FindChild(sliderData.GodotSliderLabelName, true, false) as RichTextLabel;
            var godotSlider = FindChild(sliderData.GodotSliderName, true, false) as Godot.Slider;
            godotSlider.MinValue = sliderData.MinVal;
            godotSlider.MaxValue = sliderData.MaxVal;
            godotSlider.SetMeta(SliderNumberMetaID, sliderData.Number);
            godotSlider.SetMeta(SliderLabelMetaID, label);
            if((bool)godotSlider.GetMeta(SliderIsCallbackRegistered) != true)
            {
                godotSlider.SetMeta(SliderIsCallbackRegistered, true);
                godotSlider.ValueChanged += (newVal) =>
                {
                    SetSliderLabelText(label, newVal);
                    ViewModel.SetSliderValue(sliderData.Number, newVal);
                };
            }

            godotSlider.Value = sliderData.Value;
            SetSliderLabelText(label, sliderData.Value);
            godotSlider.Step = (sliderData.MaxVal - sliderData.MinVal) / sliderData.Steps; // step is the distance between two steps in value
            this.Sliders.Add(godotSlider);
        }

        private void SetSliderLabelText(RichTextLabel label, double newVal) => label.Text = $"[center]{newVal:F2}";
        private void FindAndCreateLightOverlays()
        {
            FindOverlayBlueprint();
            this.CheckForNull(x => x.OverlayBluePrint);
            OverlayBluePrint.Hide();
            OverlayRed = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Red.ToGodotColor());
            OverlayGreen = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Green.ToGodotColor());
            OverlayBlue = DuplicateAndConfigureOverlay(OverlayBluePrint, LightColor.Blue.ToGodotColor());

            OverlaySprites.Add(OverlayRed);
            OverlaySprites.Add(OverlayGreen);
            OverlaySprites.Add(OverlayBlue);
        }

        protected Sprite2D DuplicateAndConfigureOverlay(Sprite2D overlayDraft, Godot.Color laserColor)
        {
            var newOverlay = (Sprite2D) overlayDraft.Duplicate();
            overlayDraft.GetParent().AddChild(newOverlay);
            newOverlay.Hide();
            newOverlay.Material = LightOverlayShader.Duplicate() as ShaderMaterial;
            (newOverlay.Material as ShaderMaterial).SetShaderParameter("laserColor", laserColor);
            return newOverlay;
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
        protected void ShowAndAssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderSlotNumber)
        {
            var inFlowDataAndPosition = new Godot.Vector4((float)lightAtPin.lightInFlow.Magnitude, (float)lightAtPin.lightInFlow.Phase, 0, 0);
            var outFlowDataAndPosition = new Godot.Vector4((float)lightAtPin.lightOutFlow.Magnitude, (float)lightAtPin.lightOutFlow.Phase, 0, 0);

            var overlaySets = new Godot.Collections.Array
            {
                new Godot.Collections.Dictionary
                {
                    {"overlay", slot?.BaseOverlaySprite},
                    {"name", ShaderParameterNames.LightInFlow + shaderSlotNumber},
                    {"value", inFlowDataAndPosition}
                },
                    new Godot.Collections.Dictionary
                {
                    {"overlay", slot?.BaseOverlaySprite},
                    {"name", ShaderParameterNames.LightOutFlow + shaderSlotNumber},
                    {"value", outFlowDataAndPosition}
                },
                    new Godot.Collections.Dictionary
                {
                    {"overlay", slot?.BaseOverlaySprite},
                    {"name", ShaderParameterNames.Animation + shaderSlotNumber},
                    {"value", slot.Texture}
                },
                    new Godot.Collections.Dictionary
                {
                    {"overlay", slot?.BaseOverlaySprite},
                    {"name", ShaderParameterNames.LightColor},
                    {"value", slot.MatchingLaser.Color.ToGodotColor()}
                }
            };
            if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
            {
                shaderMat.SetShaderParameter(ShaderParameterNames.LightInFlow + shaderSlotNumber, inFlowDataAndPosition);
            }
            CallDeferred(nameof(ShowAndSetShaderParameterDeferred), overlaySets);
        }

        private void ShowAndSetShaderParameterDeferred(Godot.Collections.Array overlaySets)
        {
            if (GameManager.instance.Grid.IsLightOn == false) return;
            foreach (Godot.Collections.Dictionary overlaySet in overlaySets)
            {
                var overlay = overlaySet["overlay"].As<Sprite2D>();
                var name = overlaySet["name"].ToString();
                var value = overlaySet["value"];

                if (overlay?.Material is ShaderMaterial shaderMat)
                {
                    shaderMat.SetShaderParameter(name, value);
                }
            }
            OverlayRed?.Show();
            OverlayGreen?.Show();
            OverlayBlue?.Show();
        }
        public void HideLightVector() {
            OverlayRed?.Hide();
            OverlayGreen?.Hide();
            OverlayBlue?.Hide();
            ResetAllShaderParametersToZero();
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
                    ViewModel.DeleteComponentCommand?.ExecuteAsync(new DeleteComponentArgs(ViewModel.GridX, ViewModel.GridY)).RunSynchronously();
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    var args = new RotateComponentArgs(ViewModel.GridX, ViewModel.GridY);
                    if (ViewModel.RotateComponentCommand?.CanExecute(args) == true)
                    {
                        ViewModel.RotateComponentCommand?.ExecuteAsync(args).RunSynchronously();
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
            var copy = (ComponentView)base.Duplicate();
            copy.Initialize(AnimationSlotRawData, WidthInTiles, HeightInTiles);
            copy._Ready();
            copy.ViewModel = new ComponentViewModel();
            copy.ViewModel.RotationCC = ViewModel.RotationCC; // give the new copy the proper RotationCC so that it has the correct rotation

            // deep copy that list of sliders
            List<SliderViewData> newSliderData = DuplicateSliders();
            copy.ViewModel.InitializeComponent(ViewModel.TypeNumber, newSliderData, ViewModel.Logger);
            return copy;
        }

        private List<SliderViewData> DuplicateSliders()
        {
            var newSliderData = new List<SliderViewData>();
            foreach (var slider in ViewModel.SliderData)
            {
                newSliderData.Add(new SliderViewData()
                {
                    GodotSliderLabelName = slider.GodotSliderLabelName,
                    GodotSliderName = slider.GodotSliderName,
                    Value = slider.Value,
                    MaxVal = slider.MaxVal,
                    MinVal = slider.MinVal,
                    Number = slider.Number,
                    Steps = slider.Steps
                });
            }

            return newSliderData;
        }

        public override void _ExitTree()
        {
            ViewModel.TreeExited();
            base._ExitTree();
        }
    }
}
