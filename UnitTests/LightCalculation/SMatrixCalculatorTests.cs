using CAP_Core.ExternalPorts;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static ConnectAPIC.Scripts.Helpers.SMatrixCalculator;

namespace UnitTests.LightCalculation
{
    public partial class SMatrixCalculatorTests
    {
        public Complex[,] GetReferenceMMIMatrix()
        {

            Complex i = Complex.ImaginaryOne;

            double factor = 0.5;

            Complex[,] M = new Complex[,]
            {
                { factor * Complex.Exp(i * Math.PI / 4), -factor, factor, factor * Complex.Exp(i * Math.PI / 4) },
                { -factor, factor * Complex.Exp(i * Math.PI / 4), factor * Complex.Exp(i * Math.PI / 4), factor },
                { factor, factor * Complex.Exp(i * Math.PI / 4), factor * Complex.Exp(i * Math.PI / 4), -factor },
                { factor * Complex.Exp(i * Math.PI / 4), factor, -factor, factor * Complex.Exp(i * Math.PI / 4) }
            };

            return M;
        }
        private static bool ArePhasesEqual(double phase1, double phase2)
        {
            double diff = Math.Abs(phase1 - phase2);
            double normalizedDiff = diff % (2 * Math.PI); // Normalisiere den Unterschied im Bereich von 0 bis 2π

            // Prüft, ob der Unterschied nahe 0 oder nahe 2π ist, unter Berücksichtigung einer kleinen Toleranz
            double tolerance = 1e-10; // Toleranz für Gleitkommavergleich
            return Math.Abs(normalizedDiff) < tolerance || Math.Abs(normalizedDiff - 2 * Math.PI) < tolerance;
        }

        [Fact]
        public void TestMMICalculator4X4Dimensions()
        {
            // the MMI pin definition looks like that:
            //  0  <->  1
            //  2  <->  3
            //  4  <->  5
            //  6  <->  7
            int dimensions = 4;
            var connections = SMatrixMMICalculator.GetConnections(dimensions);
            var referenceMatrix = GetReferenceMMIMatrix();
            var json = SMatrixMMICalculator.GetConnectionsJson(connections);
            var smatrixText = SMatrixMMICalculator.GetSMatrixString(dimensions);

            // test if the MMI's complex matrix is matching with the values in the calculated MMI 4x4 Matrix
            for (int row = 0; row < dimensions; row++) // inputs
            {
                for (int column = 0; column < dimensions; column++) // outputs
                {
                    var connection = connections.Single(c => c.FromPinNr == row * 2  && c.ToPinNr == column * 2 + 1);
                    Complex reference = referenceMatrix[row, column];
                    reference.Magnitude.ShouldBe((double)connection.Magnitude ,1e-10);
                    ArePhasesEqual(reference.Phase,(double)connection.Phase).ShouldBe(true);
                }
            }
        }

        [Fact]
        public void TestMMICalculatorLightPropagation3X3()
        {
            var sMatrix = SMatrixMMICalculator.GetSMatrix(3);
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(sMatrix.RowCount);
            inputVector[0] = 0.6;
            
            var outputVector = sMatrix * inputVector;
            var outputUp = outputVector[0].Magnitude;
            
            Assert.Equal((double)outputVector[1].Magnitude , (double) inputVector[0].Magnitude / 3, (double)0.000001);
        }
    }
}
