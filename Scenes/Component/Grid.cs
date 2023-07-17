using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public class Grid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Tile[,] Tiles { get; private set; }

        public Grid(int sizeX, int sizeY)
        {
            Width = sizeX;
            Height = sizeY;
            Tiles = new Tile[sizeX, sizeY];
        }

        public bool IsColliding(int x, int y, int sizeX, int sizeY)
        {
            if (IsInGrid(x, y, sizeX, sizeY) == false)
            {
                return true;
            }

            for (int i = x; i < x + sizeX - 1; i++)
            {
                for (int j = y; j < y + sizeY - 1; j++)
                {
                    if (Tiles[i, j] != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= Width && y + height <= Height;
        }
        public ComponentBase getComponentAt(int x, int y)
        {
            if (IsInGrid(x, y, 1, 1) == false)
            {
                return null;
            }

            return Tiles[x, y].Component;
        }

        public void placeComponent(int x, int y, ComponentBase componentBlueprint)
        {
            if (IsColliding(x, y, componentBlueprint.WidthInTiles, componentBlueprint.HeightInTiles))
            {
                return;
            }

            ComponentBase item = new ComponentBase(componentBlueprint.WidthInTiles, componentBlueprint.HeightInTiles, componentBlueprint.SubTiles);

            for (int i = 0; i < componentBlueprint.WidthInTiles; i++)
            {
                for (int j = 0; j < componentBlueprint.HeightInTiles; j++)
                {
                    Tiles[x + i, y + j].Component = item;
                    Tiles[x + i, y + j].image = componentBlueprint.GetSubTileAt(i, j).image;
                }
            }
        }

        public void RemoveComponentAt(int x, int y)
        {
            ComponentBase item = getComponentAt(x, y);

            if (item == null)
            {
                return;
            }

            for (int i = 0; i < item.WidthInTiles; i++)
            {
                for (int j = 0; j < item.HeightInTiles; j++)
                {
                    Tiles[x + i, y + j] = null;
                }
            }
        }
    }
}