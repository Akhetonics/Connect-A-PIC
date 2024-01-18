using CAP_Core.Tiles.Grid;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class NonLinearMatrixTest
    {
        [Fact] 
        public void NonLinearMatrixCalculation()
        {
            List<Guid> pins = new() { Guid.NewGuid(), Guid.NewGuid() , Guid.NewGuid(), Guid.NewGuid()};
            Complex[] inputValues = new Complex[] { new(1, 0), new(0, 0) , new(0,0), new (0,0)};
            Dictionary<(Guid, Guid), Func<Complex, Complex>> nonLinearConnections = new()
            {
                { (pins[0],pins[1]) , x=> x },
                { (pins[1],pins[2]) , x=> x },
                { (pins[2],pins[3]) , x=> x },
            };

            SMatrix sMatrix = new(pins);
            MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(inputValues);
            sMatrix.SetValues(nonLinearConnections);
            sMatrix.GetLightPropagation(inputVector, )
        }
    }
}
