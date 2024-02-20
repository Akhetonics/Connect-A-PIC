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
        public SliderManager SliderManager { get; }
        public OverlayManager OverlayManager { get; set; }

        public ComponentView()
        {
            ViewModel = new ComponentViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            SliderManager = new SliderManager(ViewModel, this);
            OverlayManager = new OverlayManager(this);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComponentViewModel.RotationCC))
            {
                OverlayManager.AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(ViewModel.RotationCC)); // the slots need to know the rotation for proper animation matching
                RotationDegrees = ViewModel.RotationCC.ToDegreesClockwise();
            } 
            else if (e.PropertyName == nameof(ComponentViewModel.LightsAtPins))
            {
                var lightAndSlots = new List<(LightAtPin light, List<AnimationSlot>)>();
                var shaderSlotNumber = 1;
                
                foreach (LightAtPin light in ViewModel.LightsAtPins)
                {
                    var slots = AnimationSlot.FindMatching(OverlayManager.AnimationSlots, light);
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
            OverlayManager.Initialize(animationSlotOverlays , WidthInTiles , HeightInTiles);
        }

        protected void ShowAndAssignInAndOutFlowShaderData(AnimationSlot slot, LightAtPin lightAtPin, int shaderSlotNumber)=>
            OverlayManager.ShowAndAssignInAndOutFlowShaderData(slot,lightAtPin,shaderSlotNumber);
        
        public void HideLightVector() => OverlayManager.HideLightVector();

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
            copy.Initialize(OverlayManager.AnimationSlotRawData, WidthInTiles, HeightInTiles);
            copy._Ready();
            copy.ViewModel = new ComponentViewModel();
            copy.ViewModel.RotationCC = ViewModel.RotationCC; // give the new copy the proper RotationCC so that it has the correct rotation

            // deep copy that list of sliders
            List<SliderViewData> newSliderData = SliderManager.DuplicateSliders();
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
