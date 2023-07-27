using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Tiles
{

    public partial class Tile : TileBase
    {

        public delegate void TileEventHandler(Tile tile);
        public event TileEventHandler OnDeletionRequested;
        public event TileEventHandler OnRotationRequested;
        
        
        public override void _GuiInput(InputEvent inputEvent)
        {
            base._GuiInput(inputEvent);
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.Position.X < 0 || mouseEvent.Position.Y < 0 || mouseEvent.Position.X > Size.X || mouseEvent.Position.Y > Size.Y)
                {
                    return;
                }
                if (mouseEvent.ButtonIndex == MouseButton.Middle && mouseEvent.Pressed)
                {
                    OnDeletionRequested?.Invoke(this);
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    OnRotationRequested?.Invoke(this);
                }
            }
        }

    }
}