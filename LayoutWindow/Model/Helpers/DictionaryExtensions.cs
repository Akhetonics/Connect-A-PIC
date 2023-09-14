using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Helpers
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
    }
}
