using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;

namespace CAP_Core.Component.ComponentDraftMapper.DTOs
{
    public class Pin
    {
        public int number { get; set; }
        public string name { get; set; }
        public MatterType matterType { get; set; }
        public RectSide side { get; set; }
        public int partX { get; set; }
        public int partY { get; set; }
    }
}
