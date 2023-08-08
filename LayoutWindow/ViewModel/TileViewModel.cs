using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scenes.Tiles;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tiles;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class TileViewModel
    {
        
        private readonly Grid Grid;

        public ComponentBaseView ComponentView { get; set; }
        public Tile Tile { get; set; }
        
        public TileViewModel (Tile tile , Grid grid)
        {
            Tile = tile;
            this.Grid = grid;
            
        }
        
    }
}
