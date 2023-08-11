using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.Collections;
using Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.Component
{
    public abstract class ComponentBase
{
        public int WidthInTiles => Parts.GetLength(0);
        public int HeightInTiles => Parts.GetLength(1);
        public bool IsPlacedInGrid { get; private set; }
        public int GridXMainTile { get; protected set; }
        public int GridYMainTile { get; protected set; }
        public virtual Part[,] Parts { get; protected set; }
        public SMatrix Connections { get; protected set; }
        private DiscreteRotation _discreteRotation;
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
        protected ComponentBase()
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
        public Part GetPartAtGridXY(int gridX,int gridY)
        {
            int offsetX = gridX- GridXMainTile;
            int offsetY = gridY - GridYMainTile;
            return GetPartAt(offsetX, offsetY);
        }
        public Part GetPartAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            return Parts[offsetX, offsetY];
        }
        public Guid PinIdRight(int offsetX, int offsetY)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectangleSide.Right).ID;
        }
        public Guid PinIdDown(int offsetX, int offsetY)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectangleSide.Down).ID;
        }
        public Guid PinIdLeft(int offsetX, int offsetY)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectangleSide.Left).ID;
        }
        public Guid PinIdUp(int offsetX, int offsetY)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectangleSide.Up).ID;
        }
        public abstract string NazcaFunctionName { get; set; }
    }
}