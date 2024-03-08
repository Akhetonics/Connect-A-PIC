using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using CAP_Core.Components.ComponentHelpers;

namespace ConnectAPIC.Scripts.Helpers
{
    public partial class SMatrixCalculator
    {
        public class SMatrix3_3MMICalculator
        {

            private static Complex CalculateMatrixElementNew(int i_row, int j_column, int waveLengthNMInVacuum)
            {
                i_row = 3 - i_row;
                double phaseShift;
                double Phi0 = 0;
                if (waveLengthNMInVacuum != 0)
                {
                    var specificModeRefractionIndex0 = 1;
                    var specificModeRefractionIndex1 = 1;
                    var distributionConstantBeta0 = Math.PI * 2 * specificModeRefractionIndex0 / waveLengthNMInVacuum;
                    var distributionConstantBeta1 = Math.PI * 2 * specificModeRefractionIndex1 / waveLengthNMInVacuum;
                    var LMMI = Math.PI / (distributionConstantBeta0 - distributionConstantBeta1);
                    Phi0 = -distributionConstantBeta0 * LMMI;
                }
                
                if ((j_column + i_row) % 2 == 0)
                {
                    phaseShift = Phi0+ Math.PI + Math.PI / 16 * (j_column - i_row) * (8-j_column + i_row);
                }
                else
                {
                    phaseShift = Phi0 + Math.PI / 16 * (i_row + j_column - 1) * (8 - j_column - i_row +1);
                }
                return Complex.FromPolarCoordinates(1/Math.Sqrt(3) , phaseShift);
            }

            public static string GetSMatrixString(int waveLengthNMInVacuum)
            {
                string smatrixText = "";
                for (int row = 1; row <= 3; row++) // inputs
                {
                    for (int column = 1; column <= 3; column++) // outputs
                    {
                        Complex result = CalculateMatrixElementNew(row, column, waveLengthNMInVacuum);
                        // also create the smatrix for reference
                        smatrixText += $"(r:{result.Magnitude:F4},ϕ:{result.Phase:F4})\t";
                    }
                    smatrixText += "\n";
                }
                return smatrixText;
            }
            public static Matrix<Complex> GetSMatrix(int waveLengthNMInVacuum)
            {
                // Create and initialize the matrix with size dimensions x dimensions
                Matrix<Complex> SMat = Matrix<Complex>.Build.Dense(3, 3, (row, column) =>
                {
                    // Adjusting the indices by 1, as MathNet.Numerics starts at 0, while your implementation starts at 1
                    return CalculateMatrixElementNew(row + 1, column + 1, waveLengthNMInVacuum);
                });

                return SMat;
            }

            public static string GetConnectionsJson(List<Connection> connections )
            {
                // Convert connections listto JSON
                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(connections, options);
                return json;
            }
            public static List<Connection> GetConnections(int waveLengthNMInVacuum)
            {
                List<Connection> connections = new();
                for (int row = 1; row <= 3; row++) // inputs
                {
                    for (int column = 1; column <= 3; column++) // outputs
                    {
                        Complex result = CalculateMatrixElementNew(row, column, waveLengthNMInVacuum);

                        connections.Add(new Connection
                        {
                            FromPinNr = row * 2 - 2,
                            ToPinNr = column * 2 - 1,
                            Magnitude = result.Magnitude,
                            Phase = result.Phase
                        });

                        connections.Add(new Connection
                        {
                            FromPinNr = column * 2 - 1,
                            ToPinNr = row * 2 - 2,
                            Magnitude = result.Magnitude,
                            Phase = result.Phase
                        });
                    }
                }
                return connections;
            }
        }



    }
}
