using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;

public partial class Grid : GridContainer
{
    public Tile[,] Tiles { get; private set; }
    public static int MaxTileCount { get; private set; }
    public override void _Ready()
	{
        MaxTileCount = Columns * Columns;
        Tiles= new Tile[Columns, Columns];
        RemoveAllTiles();
        CreateEmptyField();
	}

	public void CreateEmptyField()
    {
		var defaultTile = this.GetParent().GetNode<Tile>("TileTemplate");
        if (defaultTile == null) throw new ArgumentException("TileTemplate is not set in PICArea. Please tell the developer to fix that.");
        for (int i = 0; i < MaxTileCount; i++)
        {
			var newTile = defaultTile.Duplicate() as Tile;
            newTile.Visible = true;
            this.AddChild(newTile);
            Tiles[i % this.Columns, i / this.Columns] = newTile;
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
    public ComponentBase getComponentAt(int x, int y)
    {
        if (IsInGrid(x, y, 1, 1) == false)
        {
            return null;
        }

        return Tiles[x, y].Component;
    }

    public void AddComponent(int x, int y, ComponentBase componentBlueprint)
    {
        if (IsColliding(x, y, componentBlueprint.WidthInTiles, componentBlueprint.HeightInTiles))
        {
            return;
        }

        ComponentBase item = new ComponentBase(componentBlueprint.WidthInTiles, componentBlueprint.HeightInTiles, componentBlueprint.SubTiles);

        for (int i = 0; i < componentBlueprint.WidthInTiles; i++)
        {
            for (int j = 0; j < componentBlueprint.HeightInTiles; j++)
            {
                Tiles[x + i, y + j].Component = item;
                Tiles[x + i, y + j].Texture = componentBlueprint.GetSubTileAt(i, j).Texture;
            }
        }
    }

    public void RemoveComponentAt(int x, int y)
    {
        ComponentBase item = getComponentAt(x, y);

        if (item == null)
        {
            return;
        }

        for (int i = 0; i < item.WidthInTiles; i++)
        {
            for (int j = 0; j < item.HeightInTiles; j++)
            {
                Tiles[x + i, y + j] = null;
            }
        }
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

	public void CanComponentBePlaced(int gridX, int gridY, ComponentBase component)
	{

	}
	public void UpdateGlobalLightDistribution()
	{

	}
}
