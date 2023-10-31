using CAP_Core.Component.ComponentHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class DictionaryExtensions
    {
        public static Complex TryGetVal(this Dictionary<Guid, Complex> lightVector, Guid key)
        {
            Complex lightVectorIn = 0;
            if (lightVector.ContainsKey(key))
            {
                lightVectorIn = lightVector[key];
            }
            return lightVectorIn;
        }
    }
}
