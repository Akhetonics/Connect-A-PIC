using System.Numerics;
using System.Text.Json.Serialization;

namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public class Connection
    {
        [JsonPropertyName("fromPinNr")]
        public int FromPinNr { get; set; }
        [JsonPropertyName("toPinNr")]
        public int ToPinNr { get; set; }
        [JsonPropertyName("nonLinearFormula")]
        public string? Formula { get; set; }
        [JsonPropertyName("real")]
        public double? Real { get; set; }
        [JsonPropertyName("imaginary")]
        public double? Imaginary { get; set; }
        [JsonPropertyName("phase")]
        public double? Phase { get; set; }
        [JsonPropertyName("magnitude")]
        public double? Magnitude { get; set; }
        public Complex ToComplexNumber()
        {
            if (Real.HasValue && Imaginary.HasValue)
            {
                return new Complex(Real.Value, Imaginary.Value);
            }
            else if (Magnitude.HasValue && Phase.HasValue)
            {
                return Complex.FromPolarCoordinates(Magnitude.Value, Phase.Value);
            }
            else if (!string.IsNullOrWhiteSpace(Formula))
            {
                return 0;
            }
            else
            {
                throw new InvalidOperationException("Not enough data to create a complex number for the connection");
            }
        }
    }
}
