using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : Node2D
    {
        public int WidthInTiles { get; set; }
        public int HeightInTiles { get; set; }
        public float RotationDegrees { get; set; }
        public Texture2D[,] gridTextures = new Texture2D[2, 1];
        
        public Texture2D GetTexture(int x, int y)
        {
            if (x < 0 || y < 0 || x >= gridTextures.GetLength(0) || y >= gridTextures.GetLength(1)) throw new ArgumentOutOfRangeException();
            return gridTextures[x, y];
        }
        public void Show(int screenX , int screenY)
        {
            // if we want to show some actual things instead of the grid textures only.
            base.Show();
        }
        
        public ComponentBaseView Duplicate()
        {
            var copy = base.Duplicate() as ComponentBaseView;
            copy.RotationDegrees = RotationDegrees;
            copy.gridTextures = gridTextures;
            return copy;
        }
    }
}
