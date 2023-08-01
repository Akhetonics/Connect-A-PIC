using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public interface IComponentView
    {
        public int WidthInTiles();
        public int HeightInTiles();
        public void GetTileView(int x, int y);
        public float GetRotationDegrees();
        public Texture2D GetTexture(int x , int y);
        
    }
}
