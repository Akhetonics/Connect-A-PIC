using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid.FormulaReading;
using CAP_Core.Tiles.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace CAP_Core.Components.Creation
{
    public class SMatrixFactory
    {

        public static SMatrix GetSMatrix(List<Connection> connections, List<Pin> allPinsOfComponent, double laserWaveLengthNM)
        {
            var allPinsGuids = connections.SelectMany(c => new List<Guid> { c.FromPin, c.ToPin }).Distinct().ToList();
            var componentConnections = new SMatrix(allPinsGuids);
            var connectionWeights = new Dictionary<(Guid, Guid), Complex>();
            var nonLinearConnectionFunctions = new Dictionary < (Guid PinIdStart, Guid PinIdEnd), ConnectionFunction> ();
            foreach (Connection connection in connections)
            {
                if (connection.NonLinearConnectionFunction == null) continue;
                nonLinearConnectionFunctions.Add((connection.FromPin, connection.ToPin), (ConnectionFunction) connection.NonLinearConnectionFunction);
            };

            componentConnections.SetNonLinearConnectionFunctions(nonLinearConnectionFunctions);
            componentConnections.SetValues(connectionWeights);
            return componentConnections;
        }
    }
}
