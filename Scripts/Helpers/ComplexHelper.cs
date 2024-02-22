using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class ComplexHelper
    {
        public static float NormalizePhase(this Complex complexNumber)
        {
            double phase = complexNumber.Phase;
            double normalizedPhase = (phase + Math.PI) / (2 * Math.PI);
            return (float)normalizedPhase;
        }
    }
}
