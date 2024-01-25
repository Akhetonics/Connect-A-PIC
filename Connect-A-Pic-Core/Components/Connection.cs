using CAP_Core.Grid.FormulaReading;

namespace CAP_Core.Components
{
    public class Connection
    {
        public Guid FromPin { get; set; }
        public Guid ToPin { get; set; }
        public double RealValue { get; set; }
        public ConnectionFunction? NonLinearConnectionFunction { get; set; }
        public double Imaginary { get; set; }
        public string? NonLinearFunctionRaw { get; set; }
    }
}
