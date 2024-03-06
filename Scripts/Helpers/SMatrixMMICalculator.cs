using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace ConnectAPIC.Scripts.Helpers
{
    public partial class SMatrixCalculator
    {
        public class SMatrixMMICalculator
        {

            private static Complex CalculateMatrixElementNew(int s_row, int t_column, int dimensions)
            {
                Complex i = Complex.ImaginaryOne;
                Complex alpha;
                if ((t_column - s_row) % 2 == dimensions % 2)
                {
                    alpha = 1 / Math.Sqrt(dimensions) * Complex.Exp(i * Math.PI / dimensions * (1 - Math.Pow(t_column - s_row, 2) / dimensions));
                }
                else
                {
                    alpha = -1 / Math.Sqrt(dimensions) * Complex.Exp(i * Math.PI / dimensions * (1 - Math.Pow(t_column + s_row - 1, 2) / dimensions));
                }
                return alpha;
            }

            public static string GetSMatrixString(int dimensions)
            {
                string smatrixText = "";
                for (int row = 1; row <= dimensions; row++) // inputs
                {
                    for (int column = 1; column <= dimensions; column++) // outputs
                    {
                        Complex result = CalculateMatrixElementNew(row, column, dimensions);
                        // also create the smatrix for reference
                        smatrixText += $"(r:{result.Magnitude:F4},ϕ:{result.Phase:F4})\t";
                    }
                    smatrixText += "\n";
                }
                return smatrixText;
            }
            public static Matrix<Complex> GetSMatrix(int dimensions)
            {
                // Create and initialize the matrix with size dimensions x dimensions
                Matrix<Complex> SMat = Matrix<Complex>.Build.Dense(dimensions, dimensions, (row, column) =>
                {
                    // Adjusting the indices by 1, as MathNet.Numerics starts at 0, while your implementation starts at 1
                    return CalculateMatrixElementNew(row + 1, column + 1, dimensions);
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
                for (int row = 1; row <= dimensions; row++) // inputs
                {
                    for (int column = 1; column <= dimensions; column++) // outputs
                    {
                        Complex result = CalculateMatrixElementNew(row, column, dimensions);

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
