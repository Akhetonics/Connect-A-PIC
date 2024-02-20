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
        private Node2D RotationArea { get; set; } // the part of the component that rotates
        public Sprite2D OverlayBluePrint { get; set; }
        public ComponentViewModel ViewModel { get; private set; }
        public new float RotationDegrees{
            get => RotationArea?.RotationDegrees ?? 0;
            set{
                if (RotationArea?.RotationDegrees != null)
                    RotationArea.RotationDegrees = value;
            }
        }
        private new float Rotation { get => RotationArea.Rotation; set => RotationArea.Rotation = value; }
        private SliderManager sliderManager { get; }
        private OverlayManager overlayManager { get; }

        public ComponentView()
        {
            ViewModel = new ComponentViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            sliderManager = new SliderManager(ViewModel, this);
            overlayManager = new OverlayManager();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComponentViewModel.RotationCC))
            {
                overlayManager.AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(ViewModel.RotationCC)); // the slots need to know the rotation for proper animation matching
                RotationDegrees = ViewModel.RotationCC.ToDegreesClockwise();
            } 
            else if (e.PropertyName == nameof(ComponentViewModel.LightsAtPins))
            {
                var lightAndSlots = new List<(LightAtPin light, List<AnimationSlot>)>();
                var shaderSlotNumber = 1;
                
                if(ViewModel.LightsAtPins.FirstOrDefault().lightType == LaserType.Red)
                {
                    oldDebugLabels.ForEach(l => {
                        l.QueueFree();
                    });
                    oldDebugLabels = new();
                }
                
                foreach (LightAtPin light in ViewModel.LightsAtPins)
                {
                    var slots = AnimationSlot.FindMatching(overlayManager.AnimationSlots, light);
                    foreach(var slot in slots)
                    {
                        this.ShowAndAssignInAndOutFlowShaderData(slot, light, shaderSlotNumber);
                        shaderSlotNumber++;
                    }
                }
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
            overlayManager.Initialize(animationSlotOverlays);
        }

        protected void ShowAndAssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderSlotNumber)
        {
            overlayManager.ShowAndAssignInAndOutFlowShaderData(slot,lightAtPin,shaderSlotNumber);
        }
        List<Godot.Node> oldDebugLabels = new();
        private void CreateDebugLabel(Vector2I offset , string text)
        {
            // create the label
            var infoLabel = new Godot.Label();
            AddChild(infoLabel);


            // set the text 
            infoLabel.CustomMinimumSize = new Vector2(GameManager.TilePixelSize, 50); 
            infoLabel.GlobalPosition = new Vector2(GlobalPosition.X + offset.X , GlobalPosition.Y + offset.Y ); 
            infoLabel.Text = text;
            infoLabel.MouseFilter = MouseFilterEnum.Ignore;
            infoLabel.MoveToFront();

            // create the background
            ColorRect background = new ColorRect();
            background.Color = new Godot.Color(0.1f, 0.1f, 0.1f, 0.5f);
            background.CustomMinimumSize = infoLabel.CustomMinimumSize;
            background.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(background);
            background.GlobalPosition = infoLabel.GlobalPosition;
            infoLabel.MoveToFront();
            oldDebugLabels.Add(infoLabel);
            oldDebugLabels.Add(background);
        }

        public void HideLightVector() => overlayManager.HideLightVector();

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
            copy.Initialize(overlayManager.AnimationSlotRawData, WidthInTiles, HeightInTiles);
            copy._Ready();
            copy.ViewModel = new ComponentViewModel();
            copy.ViewModel.RotationCC = ViewModel.RotationCC; // give the new copy the proper RotationCC so that it has the correct rotation

            // deep copy that list of sliders
            List<SliderViewData> newSliderData = sliderManager.DuplicateSliders();
            copy.ViewModel.InitializeComponent(ViewModel.TypeNumber, newSliderData, ViewModel.Logger);
            return copy;
        }

        

        public override void _ExitTree()
        {
            ViewModel.TreeExited();
            base._ExitTree();
        }
    }
}
