using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;

namespace ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs
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
