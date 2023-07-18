using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class TemplateTile : Tile
{
    [Export] public NodePath componentTemplatePath;
    public ComponentBase componentTemplate;
    public override bool _CanDropData(Vector2 position, Variant data)
	{
        return false;
	}
	
	public override void _DropData(Vector2 position, Variant data)
	{
        // here you can drop either Tiles that have a component which would then be moved if it fits in here
        // or you can add Components which would be instantiated if they fit in here. 
        // we really need the gameManager to do that properly.
		GD.Print("Dropping");
        if(data.Obj is Tile tile)
        {
            this.Texture = tile.Texture;
        } else if (data.Obj is ComponentBase component)
        {
            if (GameManager.Instance.Grid.CanComponentBePlaced(GridX, GridY, component))
            {
                GameManager.Instance.Grid.AddComponent(GridX, GridY, component);
            }
        }
        
    }
	
	public override Variant _GetDragData(Vector2 position)
	{
		return componentTemplate;
	}
}
