using CAP_Core.Tiles;
using Godot;

namespace ConnectAPIC.LayoutWindow.View
{
	public record AnimationSlotOverlayData
	{
		public string LightFlowOverlayPath { get; set; }
		public RectSide Side;
		public int OffsetX;
		public int OffsetY;
	}
}
