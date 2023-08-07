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
        public ICommand CreateComponentCommand { get; set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        private readonly Grid Grid;

        public ComponentBaseView ComponentView { get; set; }
        public Tile Tile { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public void SetPositionInGrid(int X, int Y)
        {
            GridX = X;
            GridY = Y;
            
        }
        public TileViewModel (Tile tile , Grid grid)
        {
            Tile = tile;
            this.Grid = grid;
            CreateComponentCommand = new CreateComponentCommand(Grid, GridX, GridY);
            DeleteComponentCommand = new DeleteComponentCommand(Grid, GridX, GridY);
            RotateComponentCommand = new RotateComponentCommand(Grid, GridX, GridY);
        }
        
    }
}
