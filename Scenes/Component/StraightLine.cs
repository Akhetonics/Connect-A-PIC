using ConnectAPIC.Scenes.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.Component
{
    public partial class StraightLine : ComponentBase
    {
        public StraightLine(int WidthInTiles, int HeightInTiles, Tile[,] subTiles) : base(WidthInTiles, HeightInTiles, subTiles)
        {
            
        }
    }
}
