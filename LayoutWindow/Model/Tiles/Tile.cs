using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.Scenes.Compiler;
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
            return $"cell_{gridX}_{gridY}";
        }
        public string ExportToNazca(Tile parentTile, string parameters = "")
        {
            var parentCellName = getNazcaCellName(parentTile.GridX, parentTile.GridY);
            var parentTileEdgeSide = GetParentTileTouchingEdgeSide(parentTile);
            var parentPinName = parentTile.GetPinAt(parentTileEdgeSide).Name;
            return ExportToNazcaExtended(new IntVector(parentTile.GridX, parentTile.GridY), parentCellName, parentPinName, parameters);
        }
        public string ExportToNazcaExtended(IntVector parentGridPos, string parentCellName, string parentPinName, string parameters ="")
        {
            var cellName = getNazcaCellName(this.GridX, GridY);
            var currentDirection = (IntVector)GetParentTouchingEdgeSide(parentGridPos.X, parentGridPos.Y, GridX, GridY) * (-1);
            var currentPinName = GetPinAt(currentDirection).Name;
            return $"{cellName} = {NazcaCompiler.PDKName}.{Component.NazcaFunctionName}({parameters}).put('{currentPinName}', {parentCellName}.pin['{parentPinName}'])\n";
        }
        private RectangleSide GetParentTileTouchingEdgeSide(Tile parentTile)
        {
            return GetParentTouchingEdgeSide(parentTile.GridX, parentTile.GridY, GridX, GridY);
        }
        public static RectangleSide GetParentTouchingEdgeSide(int parentx, int parenty , int childx, int childy)
        {
            int xdir = Math.Clamp(parentx - childx, -1, 1);
            int ydir = Math.Clamp(parenty - childy, -1, 1);
            var lightFLowDirection = new IntVector(xdir, ydir);
            return (RectangleSide)lightFLowDirection;
        }
        public Pin GetPinAt(RectangleSide side)
        {
            if (Component == null) return null;
            var Part = Component.GetPartAtGridXY(GridX, GridY);
            if (Part == null) return null;
            return Part.GetPinAt(side);
        }

    }
}