namespace CAP_Core.Components
{
    public class Connection
    {
        public Guid FromPin { get; set; }
        public Guid ToPin { get; set; }
        public float Magnitude { get; set; }
        public float WireLengthNM { get; set; }
    }
}
