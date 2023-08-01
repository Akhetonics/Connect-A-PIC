using ConnectAPIC.LayoutWindow.View;
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

    public void SetTilesTexture(int x, int y, (Texture2D,float)[,] TextureAndRotationDegrees)
    {
        for (int i = 0; i < TextureAndRotationDegrees.GetLength(0); i++)
        {
            for (int j = 0; j < TextureAndRotationDegrees.GetLength(1); j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                (Texture2D texture, float rotation) = TextureAndRotationDegrees[i, j];
                SetTileTexture(gridX, gridY, texture, rotation);
            }
        }
    }
    public void SetTileTexture(int x , int y , Texture2D texture , float rotationDegrees)
    {
        if (IsInGrid(x, y, 1, 1) == false) return;
        TileViews[x, y].ResetToDefault(DefaultTile.Texture);
        TileViews[x, y].AddChild(DefaultTile.Duplicate());
        TileViews[x, y].Texture = texture;
        TileViews[x, y].RotationDegrees = rotationDegrees;
    }

    public void CreateComponentView(int x, int y, IComponentView componentView)
    {
        int width = componentView.WidthInTiles();
        int height = componentView.HeightInTiles();
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                SetTileTexture(gridX, gridY, componentView.GetTexture(i, j), componentView.GetRotationDegrees()); 
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
