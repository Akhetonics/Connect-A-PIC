using System.Linq.Dynamic.Core;
using System.Reflection;

namespace CAP_Core.Grid.FormulaReading
{
    public static class ParameterIdentifiers
    {
        public const string PinParameterIdentifier = "PIN";
        public const string SliderParameterIdentifier = "SLIDER";
        // add more constants here if needed

        public static readonly string[] All = typeof(ParameterIdentifiers)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => (string)fi.GetValue(null))
            .ToArray();
    }
}