using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : TextureRect
    {
        [Export] public int WidthInTiles { get; private set; }
        [Export] public int HeightInTiles { get; private set; }
        
        public GridViewModel ViewModel { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        public abstract void DisplayLightVector(List<LightAtPin> lightsAtPins);
        public abstract void HideLightVector();
        
        public void Initialize(int gridX, int gridY, DiscreteRotation discreteRotation, GridViewModel viewModel)
        {
            if (WidthInTiles == 0) throw new Exception("width in tiles cannot be 0");
            if (HeightInTiles == 0) throw new Exception("height in tiles cannot be 0");
            this.GridX = gridX;
            this.GridY = gridY;
            this.ViewModel = viewModel;
            this.RotationDegrees = discreteRotation.ToDegrees();

            Position = new Vector2( this.GridX * TileView.TilePixelSize , this.GridY * TileView.TilePixelSize );
            switch (discreteRotation)
            {
                case DiscreteRotation.R90: // Assuming you have a corresponding enumeration value
                    Position += new Vector2(WidthInTiles * TileView.TilePixelSize,0);
                    break;
                case DiscreteRotation.R180:
                    Position += new Vector2(WidthInTiles * TileView.TilePixelSize, HeightInTiles * TileView.TilePixelSize);
                    break;
                case DiscreteRotation.R270:
                    Position += new Vector2(0, HeightInTiles * TileView.TilePixelSize);
                    break;
            }
            Visible = true;
        }
        public override void _GuiInput(InputEvent inputEvent)
        {
            base._GuiInput(inputEvent);
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                if ( mouseEvent.Position.X < 0
                    || mouseEvent.Position.Y < 0
                    || mouseEvent.Position.X > this.WidthInTiles*TileView.TilePixelSize
                    || mouseEvent.Position.Y > this.HeightInTiles * TileView.TilePixelSize)
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
        public ComponentBaseView Duplicate()
        {
            var copy = base.Duplicate() as ComponentBaseView;
            return copy;
        }
    }
}
