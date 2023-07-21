using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;

public partial class Grid : GridContainer
{
    public Tile[,] Tiles { get; private set; }
    public Tile defaultTile;
    public static int MaxTileCount { get; private set; }
    public override void _Ready()
	{
        defaultTile = this.GetParent().GetNode<Tile>("TileTemplate");
        MaxTileCount = Columns * Columns;
        Tiles = new Tile[Columns, Columns];
        RemoveAllTiles();
        CreateEmptyField();
	}

	public void CreateEmptyField()
    {
        if (defaultTile == null) throw new ArgumentException("TileTemplate is not set in PICArea. Please tell the developer to fix that.");
        for (int i = 0; i < MaxTileCount; i++)
        {
            int gridX = i % this.Columns;
            int gridY = i / this.Columns;
            var newTile = defaultTile.Duplicate();
            newTile._Ready();
            newTile.Visible = true;
            newTile.SetPositionInGrid(gridX, gridY);
            this.AddChild(newTile);
            Tiles[gridX, gridY] = newTile;
            Tiles[gridX, gridY].OnDeletionRequested += Grid_OnDeletionRequested;
            Tiles[gridX, gridY].OnRotationRequested += (Tile tile) => tile.Component?.RotateBy90();
            Tiles[gridX, gridY].OnCreateNewComponentRequested += Grid_OnCreateNewComponent;
            Tiles[gridX, gridY].OnMoveComponentRequested += Grid_OnMoveExistingComponent;
        }
    }

    private void RemoveAllTiles()
    {
		var i = 0;
        foreach (Node n in this.FindChildren("*"))
        {
			RemoveComponentAt(i % this.Columns, i / this.Columns); 
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

        for (int i = x; i < x + sizeX - 1; i++)
        {
            for (int j = y; j < y + sizeY - 1; j++)
            {
                if (Tiles[i, j] != null)
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

    public void InstantiateComponentFromBlueprint(int x, int y, Type componentType)
    {
        ComponentBase item = ComponentFactory.Instance.CreateComponent(componentType);
        if (IsColliding(x, y, item.WidthInTiles, item.HeightInTiles))
        {
            item.QueueFree();
            return;
        }
        item._Ready();
        item.RegisterPositionInGrid(x, y);
        for (int i = 0; i < item.WidthInTiles; i++)
        {
            for (int j = 0; j < item.HeightInTiles; j++)
            {
                int gridX = x + i;
                int gridY = y + j;
                Tiles[gridX, gridY].ResetToDefault(defaultTile.Texture);
                Tiles[gridX, gridY].Component = item;
                Tiles[gridX, gridY].Texture = item.GetSubTileAt(i, j).Texture;
                item.RegisterTileAsSubtile(Tiles[gridX, gridY], i, j);
            }
        }
    }
    private void Grid_OnDeletionRequested(Tile tile)
    {
        if (tile.Component != null)
            RemoveComponentAt(tile.Component.GridXMainTile, tile.Component.GridYMainTile);
    }
    private void Grid_OnCreateNewComponent(Tile tile, ComponentBase componentBlueprint)
    {
        if (CanComponentBePlaced(tile.GridX, tile.GridY, componentBlueprint))
        {
            InstantiateComponentFromBlueprint(tile.GridX, tile.GridY, componentBlueprint.GetType());
        }
    }

    private void Grid_OnMoveExistingComponent(Tile tile, ComponentBase component)
    {
        if (CanComponentBePlaced(tile.GridX, tile.GridY, component))
        {
            RemoveComponentAt(component.GridXMainTile, component.GridYMainTile);
            InstantiateComponentFromBlueprint(tile.GridX, tile.GridY, component.GetType());
        }
    }

    public void RemoveComponentAt(int x, int y)
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
                Tiles[x + i, y + j].ResetToDefault(defaultTile.Texture);
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

	public void ConnectAdjacentComponents()
	{

	}
	private void ConnectComponentToAdjacentComponents (ComponentBase component) { 
	}

	public bool CanComponentBePlaced(int gridX, int gridY, ComponentBase component)
	{
        return ! IsColliding(gridX, gridY, component.WidthInTiles, component.HeightInTiles);
	}
	public void UpdateGlobalLightDistribution()
	{

	}
}
