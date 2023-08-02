using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.TransferFunction
{
    public class GridToSMatrixConverter
    {
        private readonly Grid GridView;

        public GridToSMatrixConverter(Grid grid)
        {
            this.GridView = grid;
        }

        public Dictionary<(Pin, Pin), Complex> GetAllConnections()
        {
            int gridWidth = GridView.Tiles.GetLength(0);
            int gridHeight = GridView.Tiles.GetLength(1);
            Array allSides = Enum.GetValues(typeof(RectangleSide));
            Dictionary<(Pin, Pin), Complex> pin2pinLightDistribution = new();


            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    foreach (RectangleSide side in allSides)
                    {
                        ConnectAllComponentBorderEdges(pin2pinLightDistribution, x, y, side);
                    }
                }
            }
            return pin2pinLightDistribution;
        }

        private void ConnectAllComponentBorderEdges(Dictionary<(Pin, Pin), Complex> pin2pinLightDistribution, int x, int y, RectangleSide side)
        {
            if (IsComponentBorderEdge(x, y, side) == false) return;
            if (GridView.Tiles[x, y].Component == null) return;
            (int offsetx, int offsety) = GetOffsetByEdgeDirection(side);
            Pin currentPin = GetPin(x, y, side);
            if (IsInGrid(x + offsetx, y + offsety))
            {
                Pin foreignPin = GetPin(x+offsetx,y+offsety,side);
                pin2pinLightDistribution.Add((currentPin, foreignPin), 1);
            }
        }

        private Pin GetPin(int x, int y, RectangleSide side)
        {
            var component = GridView.Tiles[x, y].Component;
            var partx = x - component.GridXMainTile;
            var party = y - component.GridYMainTile;
            var currentPin = component.GetPartAt(partx, party).GetPinAt(side);
            return currentPin;
        }

        public bool IsInGrid(int gridx, int gridy)
        {
            if (gridx < 0 || gridy < 0 || gridx > GridView.Tiles.GetLength(0) || gridy > GridView.Tiles.GetLength(1))
            {
                return false;
            }
            return true;
        }
        public static (int, int) GetOffsetByEdgeDirection(RectangleSide side)
        {
            var offsetX = 0;
            var offsetY = 0;
            if (side == RectangleSide.Right)
            {
                offsetX = 1;
                offsetY = 0;
            }
            if (side == RectangleSide.Down)
            {
                offsetX = 0;
                offsetY = 1;
            }
            if (side == RectangleSide.Left)
            {
                offsetX = -1;
                offsetY = 0;
            }
            if (side == RectangleSide.Up)
            {
                offsetX = 0;
                offsetY = -1;
            }
            return (offsetX, offsetY);
        }
        private bool IsComponentBorderEdge(int gridx, int gridy, RectangleSide side)
        {
            (int offsetX, int offsetY) = GetOffsetByEdgeDirection(side);
            var centeredComponent = GridView.Tiles[gridx, gridy].Component;
            if (IsInGrid(gridx + offsetX, gridy + offsetY) == false)
            {
                return true;
            }
            var offsetComponent = GridView.Tiles[gridx + offsetX, gridy + offsetY].Component;
            return centeredComponent != offsetComponent;
        }
    }
}
