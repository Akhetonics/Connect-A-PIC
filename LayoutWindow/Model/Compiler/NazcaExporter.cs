using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Model;
using System.Collections.Generic;
using Tiles;

namespace ConnectAPIC.Scenes.Compiler
{
    public class NazcaExporter : IExporter
    {
        private readonly string path;
        public NazcaExporter(string path)
        {
            this.path = path;
        }
        public string CreateHeader()
        {
            return @"
                using nazca as nd;
                from TestPDK import TestPDK;

                CAPICPDK = TestPDK();

                def FullDesign(layoutName)
                {
                    using (var fullLayoutInner = nd.Cell(name: layoutName))
                    {
                        var grating = CAPICPDK.placeGrating_East(8).put(0, 0);
                    }
                }";
        }
        public string Export(Grid grid)
        {
            // Add header
            string Code = CreateHeader();
            var StartingCouplers = new List<(string, int)>() {
                ("io1",3),
                ("io2",3),
                ("io3",3),
            };
            // start at all the three inputTiles.
            foreach (StandardInput input in grid.ExternalPorts)
            {
                var y = input.TilePositionY;
                var x = 0;
                if (grid.IsInGrid(x, y, 1, 1))
                {
                    var currentComponent = grid.Tiles[x, y].Component;
                    if (currentComponent != null)
                    {
                        var MatterRight = currentComponent.Parts[0, 0].GetPinAt(RectangleSide.Right).MatterType;
                        if (MatterRight == MatterType.Light)
                        {

                        }
                        var connectedTiles = .PinIdDown(;
                    }
                }
                
                
            }
            // get their connection-Tiles (the Tile at x = 0 and y = PositionY) and check that component's outer Pins. 
            // check if that Component is Not being placed already -> if it is, break/continue.
            // Create the NazcaLine of that component with the Pin targeting the Parent's Pin -> the InputTile's_Pin
            // mark that component as "Placed" by adding it to a dictionary. 
            // if the outer Pins' Components are NOT being placed already, call the Placing Method on them, too.

            // once the whole placing is finished, we add the Footer

          
            return Code;
        }
    }
}