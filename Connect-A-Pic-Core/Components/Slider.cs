namespace CAP_Core.Components
{
    public class Slider : ICloneable
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

        // clones the object but also its ID to be exactly the ID from the old one - you have to give it a new one here.
        public object Clone()
        {
            return new Slider(ID, Number, Value, MaxValue, MinValue);
        }
    }
}