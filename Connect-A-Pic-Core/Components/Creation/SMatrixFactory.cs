using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace CAP_Core.Components.Creation
{
    public class SMatrixFactory
    {

        public static SMatrix GetSMatrix(List<Connection> connections, Part[,] parts , double laserWaveLengthNM)
        {
            var allPinsGuids = connections.SelectMany(c => new List<Guid> { c.FromPin, c.ToPin }).ToList();
            var componentConnections = new SMatrix(allPinsGuids);
            var connectionWeights = new Dictionary<(Guid, Guid), Complex>();
            foreach (Connection connection in connections)
            {
                var phaseShiftDegrees = PhaseShiftCalculator.GetDegrees(connection.WireLengthNM, laserWaveLengthNM);
                connectionWeights.Add((connection.FromPin, connection.ToPin), Complex.FromPolarCoordinates(connection.Magnitude, phaseShiftDegrees));
            };

            componentConnections.SetValues(connectionWeights);
            return componentConnections;
        }
    }
}
