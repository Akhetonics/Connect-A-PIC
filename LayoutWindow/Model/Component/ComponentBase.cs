using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Numerics;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.Component
{
    public abstract class ComponentBase
{
        public ComponentBaseView ComponentView;
        public int WidthInTiles => Parts.GetLength(0);
        public int HeightInTiles => Parts.GetLength(1);
        public bool IsPlacedInGrid { get; private set; }
        public int GridXMainTile { get; protected set; }
        public int GridYMainTile { get; protected set; }
        public virtual Part[,] Parts { get; protected set; }
        public SMatrix Connections;
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
        public ComponentBase()
        {
            Parts = new Part[1, 1];
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
            Parts = Parts.RotateClockwise();
            _discreteRotation = _discreteRotation.RotateBy90();
            foreach (Part part in Parts)
            {
                part.Rotation90 = _discreteRotation;
            }
        }
        public Part GetPartAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            return Parts[offsetX, offsetY];
        }
        public Guid PinIdRight(int x, int y)
        {
            return Parts[x, y].GetPinAt(RectangleSide.Right).ID;
        }
        public Guid PinIdDown(int x, int y)
        {
            return Parts[x, y].GetPinAt(RectangleSide.Down).ID;
        }
        public Guid PinIdLeft(int x, int y)
        {
            return Parts[x, y].GetPinAt(RectangleSide.Left).ID;
        }
        public Guid PinIdUp(int x, int y)
        {
            return Parts[x, y].GetPinAt(RectangleSide.Up).ID;
        }
    }
}