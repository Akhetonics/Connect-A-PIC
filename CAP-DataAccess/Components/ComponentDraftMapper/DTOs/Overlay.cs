using CAP_Core.Tiles;
using System.Text.Json.Serialization;

namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public class Overlay
    {
        [JsonPropertyName("overlayAnimTexturePath")]
        public string OverlayAnimTexturePath { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("rectSide")]
        public RectSide RectSide { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("flowDirection")]
        public FlowDirection? FlowDirection { get; set; }
        [JsonPropertyName("tileOffsetX")]
        public int TileOffsetX { get; set; }
        [JsonPropertyName("tileOffsetY")]
        public int TileOffsetY { get; set; }
    }
    public enum FlowDirection
    {
        Both,
        In,
        Out
    }
}
