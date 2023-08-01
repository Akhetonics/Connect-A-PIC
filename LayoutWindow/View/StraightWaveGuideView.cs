using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class StraightWaveGuideView : ComponentBaseView
    {
        [Export] Texture2D TextureLeft;
        [Export] Texture2D TextureRight;
        public Texture2D[,] textures = new Texture2D[2, 1];
        
        public StraightWaveGuideView()
        {
            textures[0, 0] = TextureLeft;
            textures[0, 1] = TextureRight;
        }
        public float GetRotationDegrees()
        {
            throw new NotImplementedException();
        }

        public Texture2D GetTexture(int x, int y)
        {
            if (x < 0 || y < 0 || x >= textures.GetLength(0) || y >= textures.GetLength(1)) throw new ArgumentOutOfRangeException();
            return textures[x, y];
        }

        public int HeightInTiles() => textures.GetLength(1);
        public int WidthInTiles() => textures.GetLength(0);

        public float SetRotationDegrees(float degrees)
        {
            throw new NotImplementedException();
        }

        public void Show(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }
    }
}
