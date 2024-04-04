using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace CAP_Core.Helpers
{
    public static class DictionaryExtensions
    {
        public static string ConvertToString(this Dictionary<(Guid, Guid), Complex> dict)
        {
            var entries = new StringBuilder();

            foreach (var kvp in dict)
            {
                entries.AppendLine($"(({kvp.Key.Item1.ToString()[..6]}, {kvp.Key.Item2.ToString()[..6]}), {kvp.Value})");
            }

            return "[" + entries + "]";
        }
        public static string ConvertToString(this Dictionary<Guid, Complex> dict)
        {
            var entries = new StringBuilder();

            foreach (var kvp in dict)
            {
                entries.AppendLine($"(({kvp.Key.ToString()[..6]}), {kvp.Value})");
            }

            return "[" + entries + "]";
        }
        public static string ToCustomString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            StringBuilder sb = new();
            foreach (var kvp in dictionary)
            {
                sb.AppendLine($"key: {kvp.Key}, val: {kvp.Value}");
            }
            return sb.ToString();
        }
    }
}
