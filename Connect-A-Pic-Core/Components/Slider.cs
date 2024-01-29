namespace CAP_Core.Components
{
    public class Slider
    {
        public Slider(Guid ID, int number, double value, double maxValue, double minValue)
        {
            this.ID = ID;
            Number = number;
            Value = value;
            MaxValue = maxValue;
            MinValue = minValue;
        }
        public Guid ID { get; set; }
        public int Number { get; set; }
        public double Value { get; set; }
        public double MaxValue{ get; set; }
        public double MinValue{ get; set; }
    }
}