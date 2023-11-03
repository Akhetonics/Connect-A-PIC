using CAP_Core.Tiles;

namespace CAP_Core.Component.ComponentDraftMapper.DTOs
{
    public class Overlay
    {
        public string overlayAnimTexturePath { get; set; }
        public RectSide rectSide { get; set; }
        public int tileOffsetX { get; set; }
        public int tileOffsetY { get; set; }
    }
}
