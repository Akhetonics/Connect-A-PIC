using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public interface ICreateComponentCommand 
    {
        public void Execute(Type componentViewType, int x , int y , DiscreteRotation rotation);
    }
    public class CreateComponentCommand : ICreateComponentCommand
    {
        public event EventHandler CanExecuteChanged;
        private Grid MainGrid;
        public CreateComponentCommand(Grid mainGrid )
        {
            this.MainGrid = mainGrid;
        }
        
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }


        public void Execute(Type componentViewType, int x , int y , DiscreteRotation rotation)
        {
            //MainGrid.Tiles[x,y].Component
        }
    }
}
