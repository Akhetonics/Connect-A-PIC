using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Godot;

namespace ConnectAPIC.LayoutWindow.View
{
    public record AnimationSlotOverlayData
    {
        public string LightFlowOverlayPath { get; set; }
        public RectSide Side;
        public FlowDirection FlowDirection;
        public int OffsetX;
        public int OffsetY;
        public AnimationSlotOverlayData(string lightFlowOverlayPath, RectSide side, FlowDirection flowDirection, int offsetX, int offsetY)
        {
            LightFlowOverlayPath = lightFlowOverlayPath;
            Side = side;
            FlowDirection = flowDirection;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
