using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public class ComponentGrid 
    {
        private class ComponentGridEntry
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Component Comp { get; set; }
            public int CompOrientation { get; set; }
            public int CompOffsetX { get; set; }
            public int CompOffsetY { get; set; }
        }

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        private List<Component> _components = new List<Component>();

        private ComponentGridEntry[,] _grid;

        public ComponentGrid(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;

            _grid = new ComponentGridEntry[SizeX, SizeY];

            // Pre-fill grid coordinates
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    _grid[i, j].X = i;
                    _grid[i, j].Y = j;
                }
            }
        }

        public bool checkCollision(int x, int y, int sizeX, int sizeY)
        {
            if (x < 0 || y < 0 || x + sizeX - 1 >= SizeX || y + sizeY - 1 > SizeY)
            {
                // Out of Bounds
                return true;
            }

            for (int i = x; i < x + sizeX - 1; i++)
            {
                for (int j = y; j < y + sizeY - 1; j++)
                {
                    if (_grid[i, j].Comp != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Component getComponentAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y > SizeY)
            {
                return null;
            }

            return _grid[x, y].Comp;
        }

        public void placeComponent(int x, int y, int orientation, Component mycomponent)
        {
            if (checkCollision(x, y, mycomponent.TileWidth, mycomponent.TileHeight))
            {
                // Out of Bounds
                return;
            }

            Component item = new Component(mycomponent.TileWidth , mycomponent.TileHeight);

            for (int i = 0; i < mycomponent.TileWidth; i++)
            {
                for (int j = 0; j < mycomponent.TileHeight; j++)
                {
                    _grid[x + i, y + j].Comp = item;
                    _grid[x + i, y + j].CompOrientation = orientation;
                    _grid[x + i, y + j].CompOffsetX = i;
                    _grid[x + i, y + j].CompOffsetY = j;
                }
            }
        }

        public void removeComponentAt(int x, int y)
        {
            Component item = getComponentAt(x, y);

            if (item == null)
            {
                return;
            }

            for (int i = 0; i < item.TileWidth; i++)
            {
                for (int j = 0; j < item.TileHeight; j++)
                {
                    _grid[x + i, y + j].Comp = null;
                    _grid[x + i, y + j].CompOrientation = 0;
                    _grid[x + i, y + j].CompOffsetX = 0;
                    _grid[x + i, y + j].CompOffsetY = 0;
                }
            }
        }
    }
}