using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.Scenes.Component
{
    public partial class StraightWaveGuide : ComponentBase
    {
        [Export] Texture2D Texture;
        public override void _Ready()
        {
            base._Ready();
            var defaultTile = GameManager.Instance.Grid.DefaultTile;
            SubTiles = new Tile[2,1];
            SubTiles[0, 0] = defaultTile.Duplicate();
            SubTiles[0, 0].Texture = Texture;
            SubTiles[0, 0].Rotation90= DiscreteRotation.R0;
            SubTiles[0, 0].InitializePin(RectangleSide.Right, "Right", MatterType.None);
            SubTiles[0, 0].InitializePin(RectangleSide.Up, "Up", MatterType.Light);
            SubTiles[0, 0].InitializePin(RectangleSide.Left, "Left", MatterType.Electricity);
            SubTiles[0, 0].InitializePin(RectangleSide.Down, "Down", MatterType.None);
            SubTiles[1, 0] = defaultTile.Duplicate();
            SubTiles[1, 0].Texture = Texture;
            SubTiles[1, 0].InitializePin(RectangleSide.Right, "1", MatterType.None);
            SubTiles[1, 0].InitializePin(RectangleSide.Up, "2", MatterType.Light);
            SubTiles[1, 0].InitializePin(RectangleSide.Left, "3", MatterType.Electricity);
            SubTiles[1, 0].InitializePin(RectangleSide.Down, "4", MatterType.None);
        }
    }
}
