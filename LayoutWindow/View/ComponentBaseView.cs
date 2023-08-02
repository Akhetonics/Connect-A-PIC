using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : Node
    {
        public int WidthInTiles { get; set; }
        public int HeightInTiles { get; set; }
        public float RotationDegrees { get; set; }
        public bool Visible{ get; set; }
        private Texture2D[,] gridTextures;
        public int GridX { get; set; }
        public int GridY { get; set; }
        protected ComponentBaseView()
        {
            gridTextures = new Texture2D[2, 1];
        }
        public Texture2D GetTexture(int gridX, int gridY)
        {
            if (gridX < 0 || gridX >= gridTextures.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(gridX));
            if (gridY < 0 || gridY >= gridTextures.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(gridY));
            return gridTextures[gridX, gridY];
        }
        public void Show(int gridX, int gridY)
        {
            this.GridX = gridX;
            this.GridY = gridY;
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
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
