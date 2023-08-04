using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using Godot;
using Model;
using System;

namespace ConnectAPic.LayoutWindow
{
    public partial class GameManager : Node
    {
        [Export] public NodePath GridViewPath { get; set; }
        public GridView GridView { get; set; }
        public Grid Grid { get; set; }
        private static GameManager instance;
        public GridViewModel GridViewModel;
        public static GameManager Instance
        {
            get { return instance; }
        }

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
                GridView = GetNode<GridView>(GridViewPath);
                Grid = new Grid(GridView.Columns, GridView.Columns);
                GridViewModel = new GridViewModel(GridView, Grid);
            }
            else
            {
                QueueFree(); // delete this object as there is already another GameManager in the scene
            }
        }
    }
}