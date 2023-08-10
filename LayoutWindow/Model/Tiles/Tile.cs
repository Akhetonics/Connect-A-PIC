using ConnectAPIC.LayoutWindow.Model.Helpers;
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
        private string getNazcaCellName(int gridX, int gridY)
        {
            return $"cell_{gridX}_{gridY}"; ;
        }
        public string ExportToNazca(Tile parentTile, string currentPinName, string parameters = "")
        {
            var parentCellName = getNazcaCellName(parentTile.GridX, parentTile.GridY);
            var cellName = getNazcaCellName(this.GridX, GridY);
            int parentPartOffsetX = parentTile.GridX - parentTile.Component.GridXMainTile;
            int parentPartOffsetY = parentTile.GridY - parentTile.Component.GridYMainTile;
            RectangleSide ParentPinDirection = RectangleSide.Right;
            if (GridX < parentTile.GridX)
            {
                ParentPinDirection = RectangleSide.Left;
            }
            else if (GridY > parentTile.GridY)
            {
                ParentPinDirection = RectangleSide.Down;
            }
            else if (GridY > parentTile.GridY)
            {
                ParentPinDirection = RectangleSide.Up;
            }
            var parentPinName = this.Component.GetPartAt(parentPartOffsetX, parentPartOffsetY).GetPinAt(ParentPinDirection).Name;
            var currentDirection = ParentPinDirection.GetOppositeDirection();
            var currentPinName = this.Component.GetPartAt()
            return $"{cellName} = CAPICPDK.{Component.NazcaFunctionName}({parameters}).put('{currentPinName}', {parentCellName}.pin['{parentPinName}'])\n";
        }

    }
}