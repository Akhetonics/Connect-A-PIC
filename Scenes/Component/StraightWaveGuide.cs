using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.Component
{
    public partial class StraightWaveGuide : ComponentBase
    {
        [Export] Texture2D Texture;

        public override void _Ready()
        {
            base._Ready();
            SubTiles = new Tile[2,1];
            SubTiles[0, 0] = new Tile();
            SubTiles[0, 0].Texture = Texture;
            SubTiles[0, 0].InitializePins(
                new Pin("b0", MatterType.Light), 
                new Pin("", MatterType.None), 
                new Pin("a0", MatterType.Light), 
                new Pin("", MatterType.None));
            SubTiles[1, 0] = new Tile();
            SubTiles[1, 0].Texture = Texture;
            SubTiles[1, 0].InitializePins(
                new Pin("b0", MatterType.Light),
                new Pin("", MatterType.None),
                new Pin("a0", MatterType.Light),
                new Pin("", MatterType.None));
        }
    }
}
