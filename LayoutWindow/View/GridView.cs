using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.ComponentModel;
using Tiles;

public partial class GridView : GridContainer
{
    public TileView[,] TileViews { get; private set; }
    public delegate void GridActionHandler(TileView tile);
    public delegate void GridActionComponentHandler(TileView tile);
    public event TileView.TileEventHandler OntileMiddleMouseClicked;
    public event TileView.TileEventHandler OnTileRightClicked;
    public event TileView.ComponentEventHandler OnNewTileDropped;
    public event TileView.ComponentEventHandler OnExistingTileDropped;
    [Export] public NodePath DefaultTilePath;
    private TileView _defaultTile;
    public int Width;
    public int Height;
    public TileView DefaultTile
    {
        get
        {
            if (_defaultTile == null)
            {
                _defaultTile = this.GetNodeOrNull<TileView>(DefaultTilePath);
            }
            return _defaultTile;
        }
    }
    public static int MaxTileCount { get; private set; }
    public void CreateEmptyField(int width , int height)
    {
        Columns = width;
        this.Width = width;
        this.Height = height;
        for (int gridX = 0; gridX < width; gridX++)
        {
            for (int gridY = 0; gridY < height; gridY++)
            {
                TileViews[gridX, gridY] = new TileView();
                TileViews[gridX, gridY].OnDeletionRequested += OntileMiddleMouseClicked;
                TileViews[gridX, gridY].OnRotationRequested += OnTileRightClicked;
                TileViews[gridX, gridY].OnCreateNewComponentRequested += OnNewTileDropped;
                TileViews[gridX, gridY].OnMoveComponentRequested += OnExistingTileDropped;
            }
        }
    }

    public void DeleteAllTiles()
    {
        var i = 0;

        foreach (TileView t in TileViews)
        {
            ResetTilesAt(i % this.Columns, i / this.Columns, 1,1);
            this.RemoveChild(t);
            i++;
        }
    }

    public bool IsInGrid(int x, int y, int width, int height)
    {
        return x >= 0 && y >= 0 && x + width <= Columns && y + height <= Columns;
    }

    public void SetTilesTexture(int x, int y, TextureRect[,] textures)
    {
        for (int i = 0; i < textures.GetLength(0); i++)
        {
            for (int j = 0; j < textures.GetLength(1); j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                if (IsInGrid(gridX, gridY, 1, 1) == false) continue;
                TileViews[gridX, gridY].ResetToDefault(DefaultTile.Texture);
                TileViews[gridX, gridY].AddChild(DefaultTile.Duplicate());
                TileViews[gridX, gridY].Texture = textures[i,j].Texture;
                TileViews[gridX, gridY].Rotation = textures[i,j].Rotation;
            }
        }
    }
    public void ResetTilesAt(int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (IsInGrid(i+x, j+y, 1,1) == false) continue;
                TileViews[x + i, y + j].ResetToDefault(_defaultTile.Texture);
            }
        }
    }
   
}
