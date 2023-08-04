using ConnectAPIC.Scenes.Component;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public abstract partial class ComponentBaseView : Node
    {
        public int WidthInTiles => Textures != null ? Textures.GetLength(0) : 0;
        public int HeightInTiles => Textures != null ? Textures.GetLength(1) : 0;
        private DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90
        {
            get => _discreteRotation;
            set
            {
                int rotationIntervals = _discreteRotation.CalculateCyclesTillTargetRotation(value);
                for (int i = 0; i < rotationIntervals; i++)
                {
                    RotateBy90();
                }
            }
        }
        public void RotateBy90()
        {
            Textures = Textures.RotateClockwise();
            _discreteRotation = _discreteRotation.RotateBy90();
        }
        public bool Visible{ get; set; }
        protected Texture2D[,] Textures;
        public int GridX { get; set; }
        public int GridY { get; set; }
        protected ComponentBaseView()
        {
            Textures = new Texture2D[2, 1];
        }
        public Texture2D GetTexture(int x, int y)
        {
            if (x < 0 || x >= Textures.GetLength(0)) throw new ArgumentOutOfRangeException($"{nameof(x)} with value {x} is not in range 0-{Textures.GetLength(0)}");
            if (y < 0 || y >= Textures.GetLength(1)) throw new ArgumentOutOfRangeException($"{nameof(y)} with value {y} is not in range 0-{Textures.GetLength(1)}");
            return Textures[x, y];
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
            copy.Rotation90 = Rotation90;
            copy.Textures = new Texture2D[Textures.GetLength(0), Textures.GetLength(1)];
            for ( int i = 0; i < Textures.GetLength(0); i++)
            {
                for ( int j = 0; j < Textures.GetLength(1); j++)
                {
                    copy.Textures[i,j] = Textures[i, j].Duplicate() as Texture2D;
                }
            }
            
            return copy;
        }
    }
}
