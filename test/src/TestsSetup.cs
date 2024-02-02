using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;

namespace ConnectAPIC.test.src
{
    public class TestsSetup : TestClass
    {
        public TestsSetup(Node testScene) : base(testScene)
        {
        }

        [SetupAll]
        public async Task SetupAll()
        {
            
        }
    }
}
