using System.Text.Json.Serialization;

namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public class WaveLengthSpecificSMatrix
    {
        [JsonPropertyName("waveLength")]
        public int WaveLength { get; set; }
        [JsonPropertyName("connections")]
        public List<Connection> Connections { get; set; }
    }
}
