using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Tiles;
using TransferFunction;

namespace UnitTests
{
    public class SMatrixTests
    {
        [Fact]
        public void TestSMatrix()
        {
            Guid PinIda1 = Guid.NewGuid();
            Guid PinIda2 = Guid.NewGuid();
            Guid PinIda3 = Guid.NewGuid();
            Guid PinIda4 = Guid.NewGuid();

            List<Guid> pinList = new() { PinIda1, PinIda2, PinIda3, PinIda4 };
            //SMatrix smatrix = new SMatrix(pinList);
            //smatrix.setValues()
        }
       
    }
}