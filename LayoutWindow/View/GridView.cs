using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.ComponentModel;
using System.Drawing;
using Tiles;

namespace ConnectAPIC.LayoutWindow.View
{

    public partial class GridView : GridContainer
    {
        public delegate void GridActionHandler(TileView tile);
        public delegate void GridActionComponentHandler(TileView tile);
        
        [Export] private NodePath DefaultTilePath;
        private TileView _defaultTile;
        public TileView DefaultTile
        {
            get
            {
                if (_defaultTile != null) return _defaultTile;
                _defaultTile = this.GetNode<TileView>(DefaultTilePath);
                return _defaultTile;
            }
        }

        public GridView()
        {
            
            if (string.IsNullOrEmpty(DefaultTilePath))
            {
                GD.PrintErr($"{nameof(DefaultTilePath)} is not assigned");
            }
        }

        
    }
}