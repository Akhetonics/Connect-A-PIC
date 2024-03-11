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
            
            public static Complex CalculateMatrixElement(double s_row, double t_column, int dimensions )
            {
                double exp;
                double deltaPhi = 0.0;// 2.0 / 12.0;
                double basis;
                double factor;
                if ((t_column - s_row) % 2 == 4 % 2)
                {
                    basis = t_column - s_row;
                    factor = (1.0 - (Math.Pow(basis, 2.0) / dimensions));
                    exp = factor / 4.0 + deltaPhi;
                }
                else
                {
                    basis = t_column + s_row - 1.0;
                    factor = (1.0 - (Math.Pow(basis, 2.0) / dimensions));
                    exp = (factor / 4.0) + 1.0 + deltaPhi;
                }
                var output = Complex.FromPolarCoordinates(1.0 / Math.Sqrt(dimensions), exp * Math.PI);
                return output;
            }

            public static string GetSMatrixString(int dimensions)
            {
                string sMatrixText = "";
                for (int row = 1; row <= dimensions; row++) // inputs
                {
                    for (int column = 1; column <= dimensions; column++) // outputs
                    {
                        Complex result = CalculateMatrixElement(row, column, dimensions);
                        // also create the smatrix for reference
                        sMatrixText += $"(r:{result.Magnitude:F4},ϕ: PI*{result.Phase :F4})\t";
                    }
                    sMatrixText += "\n";
                }
                return sMatrixText;
            }
            public static Matrix<Complex> GetSMatrix(int dimensions)
            {
                // Create and initialize the matrix with size dimensions x dimensions
                Matrix<Complex> SMat = Matrix<Complex>.Build.Dense(3, 3, (row, column) =>
                {
                    // Adjusting the indices by 1, as MathNet.Numerics starts at 0, while your implementation starts at 1
                    return CalculateMatrixElement(row +1, column + 1, dimensions);
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
            public static List<Connection> GetConnections(int dimensions)
            {
                List<Connection> connections = new();
                for (int row = 1; row <= 3; row++) // inputs
                {
                    for (int column = 1; column <= 3; column++) // outputs
                    {
                        Complex result = CalculateMatrixElement(row, column, dimensions);

                        var columnID = column * 2 - 1;
                        var rowID = row * 2 - 2;

                        connections.Add(new Connection
                        {
                            FromPinNr = rowID,
                            ToPinNr = columnID,
                            Magnitude = result.Magnitude,
                            Phase = result.Phase
                        });

                        connections.Add(new Connection
                        {
                            FromPinNr = 5 - rowID,
                            ToPinNr = 5 - columnID,
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
