using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CAP_Core.Helpers
{
    public static class EnumerableExtensions
    {
        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerable<T> source)
        {
            return new ConcurrentBag<T>(source);
        }
    }

}
