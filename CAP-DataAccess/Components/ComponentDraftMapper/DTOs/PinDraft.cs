using CAP_Core.Components;
using CAP_Core.Tiles;

namespace Components.ComponentDraftMapper.DTOs
{
    public class PinDraft
    {
        public int number { get; set; }
        public string name { get; set; }
        public MatterType matterType { get; set; }
        public RectSide side { get; set; }
        public int partX { get; set; }
        public int partY { get; set; }
    }
}
