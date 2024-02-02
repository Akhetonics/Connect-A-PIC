using CAP_Core.Components;
using CAP_Core.Tiles;
using System.Text.Json.Serialization;

namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public class PinDraft
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("matterType")]
        public MatterType MatterType { get; set; }
        [JsonPropertyName("side")]
        public RectSide Side { get; set; }
        [JsonPropertyName("partX")]
        public int PartX { get; set; }
        [JsonPropertyName("partY")]
        public int PartY { get; set; }
    }
}
