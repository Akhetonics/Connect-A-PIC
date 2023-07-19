using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public abstract partial class ComponentBase : Node
    {
        public int WidthInTiles => SubTiles.GetLength(0);
        public int HeightInTiles => SubTiles.GetLength(1);
        public Tile[,] SubTiles { get; protected set; }

        public override void _Ready()
        {
            base._Ready();
            SubTiles = new Tile[1, 1];
        }
        public DiscreteRotation DiscreteRotation { get; private set; }

        public void RotateBy90()
        {
            SubTiles.RotateCounterClockwise();
            foreach (Tile tile in SubTiles)
            {
                tile.RotateBy90();
            }
        }
        public Tile GetSubTileAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            // Todo also take Rotation into consideration
            return SubTiles[offsetX, offsetY];
        }
    }
}