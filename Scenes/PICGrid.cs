using ConnectAPIC.Scenes.Component;
using Godot;
using System;

public partial class PICGrid : GridContainer
{
	public static int MaxTileCount { get; private set; }
    public override void _Ready()
	{
        MaxTileCount = this.Columns * this.Columns;
		RemoveAllTiles();
        CreateEmptyField();
	}

	public void CreateEmptyField()
    {
		var nodes = this.GetNode("*");
		var defaultTile = this.GetNode("PICRect");
        for (int i = 0; i <= MaxTileCount; i++)
        {
            this.AddChild(defaultTile.Duplicate());
        }
    }

    private void RemoveAllTiles()
    {
        foreach (Node n in this.FindChildren("*"))
        {
            this.RemoveChild(n);
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
