using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Tiles
{

    public class Tile 
    {
        public ComponentBase Component { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        
        public Tile(int X, int Y)
        {
            GridX = X;
            GridY = Y;
        }
        
    }
}