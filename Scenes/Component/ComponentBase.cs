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
        public bool IsPlacedInGrid { get; protected set; } = false;
        public int GridXMainTile { get; protected set; }
        public int GridYMainTile { get; protected set; }
        public virtual Tile[,] SubTiles { get; protected set; }
        public DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90
        {
            get => _discreteRotation;
            set
            {
                int rotationIntervals = _discreteRotation.CalculateCyclesTillTargetRotation(value);
                for (int i = 0; i < rotationIntervals; i++)
                {
                    RotateBy90();
                }
            }
        }
        public override void _Ready()
        {
            base._Ready();
            SubTiles = new Tile[1, 1];
            _discreteRotation = DiscreteRotation.R0;
        }
       

        public void RegisterPositionInGrid(int gridX , int gridY)
        {
            IsPlacedInGrid = true;
            this.GridXMainTile = gridX;
            this.GridYMainTile = gridY;
        }
        public void ClearGridData()
        {
            IsPlacedInGrid = false;
            this.GridXMainTile = -1;
            this.GridYMainTile = -1;
        }
        public void RotateBy90()
        {
            SubTiles = SubTiles.RotateClockwise();
            _discreteRotation = _discreteRotation.RotateBy90();
            foreach (Tile tile in SubTiles)
            {
                tile.Rotation90 = _discreteRotation;
            }
        }
        public Tile GetSubTileAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            return SubTiles[offsetX, offsetY];
        }
        public ComponentBase Duplicate()
        {
            var item = base.Duplicate() as ComponentBase;
            item.SubTiles = new Tile[SubTiles.GetLength(0),SubTiles.GetLength(1)];
            for(int x = 0; x < SubTiles.GetLength(0); x++)
            {
                for (int y = 0; y < SubTiles.GetLength(1); y++)
                {
                    item.SubTiles[x,y] = SubTiles[x, y].Duplicate();
                }
            }
            item.Rotation90 = this.Rotation90;
            return item;
        }
    }
}