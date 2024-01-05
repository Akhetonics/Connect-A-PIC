using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Helpers;
using System.Globalization;

namespace CAP_Core.Tiles
{

    public class Tile
    {
        private const string PythonFunctionIndention = "        ";
        public Component Component { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        public Tile(int X, int Y)
        {
            GridX = X;
            GridY = Y;
        }
        public string GetComponentCellName()
        {
            var mainGridX = Component.GridXMainTile;
            var mainGridY = Component.GridYMainTile;
            return $"cell_{mainGridX}_{mainGridY}";
        }
        public string ExportToNazca(Tile parentTile)
        {
            var parentCellName = parentTile.GetComponentCellName();
            var parentTileEdgeSide = GetParentTileTouchingEdgeSide(parentTile);
            var parentPinName = parentTile.GetPinAt(parentTileEdgeSide).Name;
            return ExportToNazcaExtended(new IntVector(parentTile.GridX, parentTile.GridY), parentCellName, parentPinName);
        }
        public string ExportToNazcaExtended(IntVector parentGridPos, string parentCellName, string parentPinName)
        {
            if (Component == null) return "";
            var cellName = GetComponentCellName();
            var currentDirection = (IntVector)GetParentTouchingEdgeSide(parentGridPos.X, parentGridPos.Y, GridX, GridY) * -1;
            var currentPinName = GetPinAt(currentDirection)?.Name ?? "";
            var parameters = Component.NazcaFunctionParameters;
            return $"{PythonFunctionIndention}{cellName} = {Resources.NazcaPDKName}.{Component.NazcaFunctionName}({parameters}).put('{currentPinName}', {parentCellName}.pin['{parentPinName}'])\n";
        }
        public string ExportToNazcaAbsolutePosition()
        {
            if (Component == null) return "";
            var cellName = GetComponentCellName();
            var comp = Component;
            var parameters = Component.NazcaFunctionParameters;
            var rotationDisplacementCorrectionX = 0f;
            var rotationDisplacementCorrectionY = 0f;
            var rotation = (int)comp.Rotation90CounterClock * 90;
            // the StartPin is always the local (0,0) which is the top left at Nazca
            if (rotation == 90)
            {
                rotationDisplacementCorrectionX = 0.5f;
                rotationDisplacementCorrectionY = -(comp.WidthInTiles - 0.5f);
            }
            else if (rotation == 180)
            {
                rotationDisplacementCorrectionX = comp.WidthInTiles;
                rotationDisplacementCorrectionY = -1 * (comp.HeightInTiles - 1);
            }
            else if (rotation == 270)
            {
                rotationDisplacementCorrectionX = comp.HeightInTiles - 0.5f;
                rotationDisplacementCorrectionY = 0.5f;
            }
            var posX = $"({comp.GridXMainTile}+{rotationDisplacementCorrectionX.ToString(CultureInfo.InvariantCulture)})*CAPICPDK._CellSize";
            var posY = $"({-comp.GridYMainTile}+{rotationDisplacementCorrectionY.ToString(CultureInfo.InvariantCulture)})*CAPICPDK._CellSize";

            return $"{PythonFunctionIndention}{cellName} = {Resources.NazcaPDKName}.{Component.NazcaFunctionName}({parameters}).put({posX},{posY},{rotation})\n";
        }

        private RectSide GetParentTileTouchingEdgeSide(Tile parentTile)
        {
            return GetParentTouchingEdgeSide(parentTile.GridX, parentTile.GridY, GridX, GridY);
        }
        public static RectSide GetParentTouchingEdgeSide(int parentX, int parentY, int childX, int childY)
        {
            int xDir = Math.Clamp(childX - parentX, -1, 1);
            int yDir = Math.Clamp(childY - parentY, -1, 1);
            var lightFLowDirection = new IntVector(xDir, yDir);
            return (RectSide)lightFLowDirection;
        }
        public Pin? GetPinAt(RectSide side)
        {
            if (Component == null) return null;
            var Part = Component.GetPartAtGridXY(GridX, GridY);
            if (Part == null) return null;
            return Part.GetPinAt(side);
        }

    }
}