using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using ConnectAPIC.Scenes.Compiler;
using Tiles;
using Model;
using ConnectAPIC.LayoutWindow.Model.ExternalPorts;

namespace UnitTests
{
    public class ExportNazcaTests
    {
        [Fact]
        public void NazcaCompilerTest()
        {
            Grid grid = new(24,12);
            
            NazcaCompiler exporter = new(grid);

            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler

        }

    }
}