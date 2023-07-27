using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.ComponentModel;
using Tiles;

public partial class Grid : GridContainer
{
    public Tile[,] Tiles { get; private set; }
    [Export] public NodePath DefaultTilePath;
    private Tile _defaultTile;
    public Tile DefaultTile
    {
        get
        {
            if (_defaultTile == null)
            {
                _defaultTile = this.GetNodeOrNull<Tile>(DefaultTilePath);
            }
            return _defaultTile;
        }
    }
    public static int MaxTileCount { get; private set; }
    public override void _Ready()
	{
        MaxTileCount = Columns * Columns;
        Tiles = new Tile[Columns, Columns];
        RemoveAllTiles();
        CreateEmptyField();
	}

	public void CreateEmptyField()
    {
        if (DefaultTile == null) throw new ArgumentException("TileTemplate is not set in PICArea. Please tell the developer to fix that.");
        for (int i = 0; i < MaxTileCount; i++)
        {
            int gridX = i % this.Columns;
            int gridY = i / this.Columns;
            DefaultTile._Ready();
            var newTile = (Tile)DefaultTile.Duplicate();
            newTile._Ready();
            newTile.Visible = true;
            newTile.SetPositionInGrid(gridX, gridY);
            this.AddChild(newTile);
            Tiles[gridX, gridY] = newTile;
            Tiles[gridX, gridY].OnDeletionRequested += Grid_OnDeletionRequested;
            Tiles[gridX, gridY].OnRotationRequested += Grid_OnRotationRequested;
            Tiles[gridX, gridY].OnCreateNewComponentRequested += Grid_OnCreateNewComponent;
            Tiles[gridX, gridY].OnMoveComponentRequested += Grid_OnMoveExistingComponent;
        }
    }

    private void RemoveAllTiles()
    {
		var i = 0;
        foreach (Node n in this.FindChildren("*"))
        {
			UnregisterComponentAt(i % this.Columns, i / this.Columns); 
            this.RemoveChild(n);
			i++;
        }
    }

    public bool IsColliding(int x, int y, int sizeX, int sizeY)
    {
        if (IsInGrid(x, y, sizeX, sizeY) == false)
        {
            return true;
        }

        for (int i = x; i < x + sizeX; i++)
        {
            for (int j = y; j < y + sizeY; j++)
            {
                if (Tiles[i, j]?.Component != null)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public bool IsInGrid(int x, int y, int width, int height)
    {
        return x >= 0 && y >= 0 && x + width <= Columns && y + height <= Columns;
    }
    public ComponentBase GetComponentAt(int x, int y)
    {
        if (IsInGrid(x, y, 1, 1) == false)
        {
            return null;
        }

        return Tiles[x, y].Component;
    }

    public void PlaceExistingComponent(int x , int y , ComponentBase component)
    {
        if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
        {
            throw new ComponentCannotBePlacedException(component);
        }
        component.RegisterPositionInGrid(x, y);
        for (int i = 0; i < component.WidthInTiles; i++)
        {
            for (int j = 0; j < component.HeightInTiles; j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                Part part = component.GetPartAt(i, j);
                Tiles[gridX, gridY].ResetToDefault(DefaultTile.Texture);
                Tiles[gridX, gridY].Component = component;
                Tiles[gridX, gridY].Texture = part.Texture;
                Tiles[gridX, gridY].Rotation90 = component.Rotation90;
            }
        }
    }
    public ComponentBase CreateAndPlaceComponent(int x, int y, Type componentType)
    {
        ComponentBase item = ComponentFactory.Instance.CreateComponent(componentType);
        if (IsColliding(x, y, item.WidthInTiles, item.HeightInTiles))
        {
            item.QueueFree();
            return null;
        }
        PlaceExistingComponent(x, y, item);
        return item;
    }

    private void Grid_OnRotationRequested(TileBase tile)
    {
        if (tile == null || tile.Component == null) return;

        var rotatedComponent = tile.Component;
        int x = tile.Component.GridXMainTile;
        int y = tile.Component.GridYMainTile;
        UnregisterComponentAt(tile.GridX, tile.GridY);
        rotatedComponent.RotateBy90();
        try
        {
            PlaceExistingComponent(x, y, rotatedComponent);
        }
        catch (ComponentCannotBePlacedException)
        {
            rotatedComponent.Rotation90 = rotatedComponent.Rotation90 - 1;
            PlaceExistingComponent(x, y, rotatedComponent);
        }

    }
    private void Grid_OnDeletionRequested(TileBase tile)
    {
        if (tile.Component != null)
            UnregisterComponentAt(tile.Component.GridXMainTile, tile.Component.GridYMainTile);
    }
    private void Grid_OnCreateNewComponent(TileBase tile, ComponentBase componentBlueprint)
    {
        if (CanComponentBePlaced(tile.GridX, tile.GridY, componentBlueprint))
        {
            CreateAndPlaceComponent(tile.GridX, tile.GridY, componentBlueprint.GetType());
        }
    }

    private void Grid_OnMoveExistingComponent(TileBase tile, ComponentBase component)
    {
        int oldMainGridx = component.GridXMainTile;
        int oldMainGridy = component.GridYMainTile;
        UnregisterComponentAt(component.GridXMainTile, component.GridYMainTile); // to avoid blocking itself from moving only one tile into its own subtiles
        try
        {
            PlaceExistingComponent(tile.GridX, tile.GridY, component);
        } catch (ComponentCannotBePlacedException)
        {
            PlaceExistingComponent(oldMainGridx, oldMainGridy, component);
        }
    }

    public void UnregisterComponentAt(int x, int y)
    {
        ComponentBase item = GetComponentAt(x, y);
        if (item == null)
        {
            return;
        }
        x = item.GridXMainTile;
        y = item.GridYMainTile;
        for (int i = 0; i < item.WidthInTiles; i++)
        {
            for (int j = 0; j < item.HeightInTiles; j++)
            {
                Tiles[x + i, y + j].ResetToDefault(DefaultTile.Texture);
            }
        }
        item.ClearGridData();
    }

    public void Save (string Path)
	{

	}
	
	public void Export(string Path)
	{

	}
	public bool CanComponentBePlaced(int gridX, int gridY, ComponentBase component)
	{
        return ! IsColliding(gridX, gridY, component.WidthInTiles, component.HeightInTiles);
	}
	public void UpdateGlobalLightDistribution()
	{

	}
}
