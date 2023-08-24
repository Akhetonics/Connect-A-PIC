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
        public abstract string NazcaFunctionName { get; set; }
        public abstract string NazcaFunctionParameters { get; }
        private DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90CounterClock
        {
            get => _discreteRotation;
            set
            {
                int rotationIntervals = _discreteRotation.CalculateCyclesTillTargetRotation(value);
                for (int i = 0; i < rotationIntervals; i++)
                {
                    RotateBy90CounterClockwise();
                }
            }
        }
        protected ComponentBase()
        {
            Parts = new Part[1, 1];
            _discreteRotation = DiscreteRotation.R0;
        }

        public void RegisterPositionInGrid(int gridX, int gridY)
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
        public void RotateBy90CounterClockwise()
        {
            Parts = Parts.RotateCounterClockwise();
            _discreteRotation = _discreteRotation.RotateBy90();
            foreach (Part part in Parts)
            {
                part.Rotation90 = _discreteRotation;
            }
        }
        public Part GetPartAtGridXY(int gridX, int gridY)
        {
            int offsetX = gridX - GridXMainTile;
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
        public Part CreatePart(params RectSide[] LightTransmittingSides)
        {
            var part = new Part();
            foreach (RectSide side in LightTransmittingSides)
            {
                part.InitializePin(side, null, MatterType.Light);
            }
            return part;
        }
        public Guid PinIdRight(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Right).ID;
        }
        public Guid PinIdDown(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Down).ID;
        }
        public Guid PinIdLeft(int offsetX=0, int offsetY=0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Left).ID;
        }
        public Guid PinIdUp(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Up).ID;
        }
        public override string ToString()
        {
            return $"Nazca Name: {NazcaFunctionName} \n" +
                   $"Parameters: {NazcaFunctionParameters} \n" +
                   $"Width in Tiles: {WidthInTiles} \n" +
                   $"Height in Tiles: {HeightInTiles} \n" +
                   $"Is Placed in Grid: {IsPlacedInGrid} \n" +
                   $"Grid X (Main Tile): {GridXMainTile} \n" +
                   $"Grid Y (Main Tile): {GridYMainTile} \n" +
                   $"Rotation: {Rotation90CounterClock} \n" +
                   $"Parts Length: {Parts?.Length} \n" +
                   $"Connections PinReferences Count: {Connections?.PinReference?.Count}";
        }
    }
}