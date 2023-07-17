using ConnectAPIC.Scenes.Tiles;
using Godot.Collections;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public class ComponentBase
    {
        public int WidthInTiles { get; private set; }
        public int HeightInTiles { get; private set; }
        public Tile[,] SubTiles;
        public DiscreteRotation Rotation;
        
        public ComponentBase(int widthInTiles, int heightInTiles, Tile[,] subTiles)
        {
            this.WidthInTiles = widthInTiles;
            this.HeightInTiles = heightInTiles;
            this.SubTiles = subTiles;
        }
        public Tile GetSubTileAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            // Todo also take Rotation into consideration
            return SubTiles[offsetX, offsetY];
        }
    }
}