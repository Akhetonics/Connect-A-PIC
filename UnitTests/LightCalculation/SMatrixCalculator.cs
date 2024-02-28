using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.LightCalculation
{
    public class SMatrixCalculator
    {
        [Fact]
        public void Calculate4x4MMISMatrix()
        {
            List<Connection> connections = new();
            // Define the constant q
            int MMiPortSize = 4;
            string smatrixText = "";
            // Calculate and print the matrix values
            for (int s = 1; s <= MMiPortSize; s++)
            {
                for (int t = 1; t <= MMiPortSize; t++)
                {
                    Complex result = CalculateMatrixElement(s, t, MMiPortSize) ;
                    smatrixText +=($"(r:{result.Magnitude:F4},ϕ:{result.Phase:F4})\t");
                }
                smatrixText += "\n";
            }
            smatrixText += "";
        }

        static Complex CalculateMatrixElement(int s, int t, int q)
        {
            // Complex number for i
            Complex i = new Complex(0, 1);

            // Calculate the reflection parameter sigma
            int sigma = ((t - s) % 2 == q % 2) ? 1 : 0;
            // Exponential factor
            double numerator = Math.Pow(t - (Math.Pow(-1,sigma)*s-sigma), 2);
            Complex exponentialFactor = Complex.Exp(i * Math.PI / 4 * (1 - (numerator / q)));

            // Calculate the alpha(s, t) value
            Complex alpha = (Math.Pow(-1,sigma) / Math.Sqrt(q)) * exponentialFactor ;

            return alpha;
        }
    }
}
