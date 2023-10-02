using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using ConnectAPic.LayoutWindow;
using Godot;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class PinView : TextureRect
	{
		[Export] protected Texture2D ElectricityPinTexture { get; set; }
		[Export] protected Texture2D LightPinTexture { get; set; }
		public static readonly int PinPixelSize = 6;
		public Dictionary<LightColor, System.Numerics.Complex> LightIn { get; set; }
		public Dictionary<LightColor,System.Numerics.Complex> LightOut { get; set; }
		public PinView()
		{
			SetMatterType(MatterType.None);
			LightIn = new() { { LightColor.Red, 0 } , { LightColor.Green, 0 } , { LightColor.Blue, 0 } };
			LightOut = new() { { LightColor.Red, 0 } , { LightColor.Green, 0 } , { LightColor.Blue, 0 } };
		}
		public override void _Ready()
		{
			base._Ready();
			Visible = false;
		}
		public void SetPinRelativePosition(RectSide side)
		{
			if (side == RectSide.Right)
			{
				this.Position = new Vector2(GameManager.TilePixelSize - PinPixelSize, GameManager.TilePixelSize / 2 - PinPixelSize / 2);
			}
			if (side == RectSide.Down)
			{
				Position = new Vector2(GameManager.TilePixelSize / 2 - PinPixelSize / 2, GameManager.TilePixelSize - PinPixelSize);
			}
			if (side == RectSide.Left)
			{
				Position = new Vector2(0, GameManager.TilePixelSize / 2 - PinPixelSize / 2);
			}
			if (side == RectSide.Up)
			{
				Position = new Vector2(GameManager.TilePixelSize / 2 - PinPixelSize / 2, 0);
			}
		}
		public void SetMatterType(MatterType? newMatterType)
		{
			if (Texture != null)
			{
				Visible = true;
				switch (newMatterType)
				{
					case MatterType.Electricity:
						Texture = ElectricityPinTexture;
						break;
					case MatterType.Light:
						Texture = LightPinTexture;
						break;
					case MatterType.None:
					default:
						Visible = false;
						LightIn = new() { { LightColor.Red, 0 }, { LightColor.Green, 0 }, { LightColor.Blue, 0 } };
						LightOut = new() { { LightColor.Red, 0 }, { LightColor.Green, 0 }, { LightColor.Blue, 0 } };
						break;
				}
			}
		}
		public PinView Duplicate()
		{
			var copy = base.Duplicate() as PinView;
			copy.ElectricityPinTexture = ElectricityPinTexture;
			copy.LightPinTexture = LightPinTexture;
			copy._Ready();
			copy.LightIn = new Dictionary<LightColor, System.Numerics.Complex>(LightIn);
			copy.LightOut = new Dictionary<LightColor, System.Numerics.Complex>(LightOut);
			return copy;
		}
	}
}
