using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.Component
{
    public partial class StraightWaveGuide : ComponentBase
    {
        [Export] Texture2D Texture;
        public override void _Ready()
        {
            base._Ready();
            var defaultTile = GameManager.Instance.Grid.DefaultTile;
            Parts = new Part[2,1];
            Parts[0, 0] = (Part)defaultTile.Duplicate();
            Parts[0, 0].Texture = Texture;
            Parts[0, 0].Rotation90= DiscreteRotation.R0;
            Parts[0, 0].InitializePin(RectangleSide.Right, "Right", MatterType.None);
            Parts[0, 0].InitializePin(RectangleSide.Up, "Up", MatterType.Light);
            Parts[0, 0].InitializePin(RectangleSide.Left, "Left", MatterType.Electricity);
            Parts[0, 0].InitializePin(RectangleSide.Down, "Down", MatterType.None);
            Parts[1, 0] = (Part)defaultTile.Duplicate();
            Parts[1, 0].Texture = Texture;
            Parts[1, 0].InitializePin(RectangleSide.Right, "1", MatterType.None);
            Parts[1, 0].InitializePin(RectangleSide.Up, "2", MatterType.Light);
            Parts[1, 0].InitializePin(RectangleSide.Left, "3", MatterType.Electricity);
            Parts[1, 0].InitializePin(RectangleSide.Down, "4", MatterType.None);

            Connections.Add(Parts[0, 0].GetPinAt(RectangleSide.Right));
        }
    }
}
