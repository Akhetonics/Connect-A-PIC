using CAP_Core.ExternalPorts;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using JetBrains.Annotations;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
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
            double normalizedDiff = diff % (2 * Math.PI); 
            double tolerance = 1e-10;
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
            var connections = SMatrix3_3MMICalculator.GetConnections(1550);
            var referenceMatrix = GetReferenceMMIMatrix();
            var json = SMatrix3_3MMICalculator.GetConnectionsJson(connections);
            var sMatrixText = SMatrix3_3MMICalculator.GetSMatrixString(1550);

            // test if the MMI's complex matrix is matching with the values in the calculated MMI 4x4 Matrix
            for (int row = 0; row < dimensions; row++) // inputs
            {
                for (int column = 0; column < dimensions; column++) // outputs
                {
                    var connection = connections.Single(c => c.FromPinNr == row * 2 && c.ToPinNr == column * 2 + 1);
                    Complex reference = referenceMatrix[row, column];
                    reference.Magnitude.ShouldBe((double)connection.Magnitude, 1e-10);
                    ArePhasesEqual(reference.Phase, (double)connection.Phase).ShouldBe(true);
                }
            }
        }

        [Fact]
        public void TestMMICalculatorLightPropagation3X3()
        {
            // working with this formula now: https://gigvvy.com/journals/ijase/articles/ijase-201303-11-1-031.pdf
            var sMatrix = SMatrix3_3MMICalculator.GetSMatrix(3);
            var sMatrixstring = SMatrix3_3MMICalculator.GetSMatrixString(3);
            var json = SMatrix3_3MMICalculator.GetConnectionsJson(SMatrix3_3MMICalculator.GetConnections(3));
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(sMatrix.RowCount);
            inputVector[0] = Complex.FromPolarCoordinates(1, 0);
            inputVector[1] = Complex.FromPolarCoordinates(1, 0);
            inputVector[2] = Complex.FromPolarCoordinates(1, 0);

            // Act
            var outputVector = sMatrix * inputVector;
            var outputVector2 = sMatrix * outputVector;
            var outputVector3 = sMatrix * outputVector2;
            var outputVector4 = sMatrix * outputVector3;
            var outputUp = outputVector[0].Magnitude;
            // Calculate the total energy at input and output
            double inputEnergy = inputVector.Sum(x => x.MagnitudeSquared());
            double outputEnergy = outputVector.Sum(x => x.MagnitudeSquared());
            double outputEnergy2 = outputVector2.Sum(x => x.MagnitudeSquared());
            double outputEnergy3 = outputVector3.Sum(x => x.MagnitudeSquared());
            // Assert
            // Check if the total energy is conserved (within a small tolerance)
            Assert.True(Math.Abs(inputEnergy - outputEnergy) < 1e-12, $"Energy conservation failed. Input energy: {inputEnergy}, Output energy: {outputEnergy}");
            Assert.True(Math.Abs(inputEnergy - outputEnergy2) < 1e-12, $"Energy conservation failed. Input energy: {inputEnergy}, Output energy: {outputEnergy2}");
            Assert.Equal((inputVector[0].Magnitude + inputVector[1].Magnitude + inputVector[2].Magnitude) / 3, outputVector[1].MagnitudeSquared(), 0.000001);
            Assert.True((outputVector2 - inputVector).MaximumMagnitudePhase().Magnitude < 1e-10);
            Assert.True((outputVector2 - inputVector).MaximumMagnitudePhase().Phase < 1e-10);
        }

        [Fact]
        public void TestCalculationFormula()
        {
            var result = SMatrix3_3MMICalculator.CalculateMatrixElement(3,2,3);

            result.Phase.ShouldBe(0);
        }
    }
}
