using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class Tile : TextureRect
{
    public ComponentBase Component;
    protected Dictionary<RectangleSide, Pin> Pins;
    protected DiscreteRotation rotation;
    protected int GridX;
    protected int GridY;

    public override void _Ready()
    {
        Pins = new Dictionary<RectangleSide, Pin>();
    }
    /// <summary>
    /// Register the Tile when it gets created
    /// </summary>
    public void RegisterMainGridXY(int X, int Y)
    {
        GridX = X;
        GridY = Y;
    }
    public void ResetToDefault(Texture2D baseTexture)
    {
        Pins = new Dictionary<RectangleSide, Pin>();
        Texture = baseTexture;
        Component = null;
    }
    public void InitializePins(Pin right, Pin up, Pin left, Pin down)
    {
        Pins = new Dictionary<RectangleSide, Pin>();
        Pins.Add(RectangleSide.Right, right);
        Pins.Add(RectangleSide.Up, up);
        Pins.Add(RectangleSide.Left, left);
        Pins.Add(RectangleSide.Down, down);
    }
    public void RotateBy90()
    {
        if (this.Component== null) return;
        this.rotation =(DiscreteRotation)(((int)(rotation + 1) % (int)(DiscreteRotation.R270)));
        RotationDegrees = (int)rotation * 90;
        // switch all pins around, so that they all go one to the left as with the rotation
        (Pins[RectangleSide.Right], Pins[RectangleSide.Up]) = (Pins[RectangleSide.Up], Pins[RectangleSide.Right]);
        (Pins[RectangleSide.Right], Pins[RectangleSide.Left]) = (Pins[RectangleSide.Left], Pins[RectangleSide.Right]);
        (Pins[RectangleSide.Right], Pins[RectangleSide.Down]) = (Pins[RectangleSide.Down], Pins[RectangleSide.Right]);
    }
    
    public Pin GetPinAt(RectangleSide side) // takes rotation into account
    {
        return Pins.GetValueOrDefault(side);
    }

	public override bool _CanDropData(Vector2 position, Variant data)
	{
		// extract all tiles from the component that is about to be dropped here at position and SetDragPreview them
        if (data.Obj is ComponentBase component)
        {
            
            for ( int x = 0; x < component.WidthInTiles; x++){
                for ( int y = 0; y < component.HeightInTiles; y++)
                {
                    var previewtile = component.GetSubTileAt(x, y).Duplicate() as Tile;
                    previewtile.Position = new Vector2(position.X + x * 64, position.Y + y * 64);
                    this.SetDragPreview(previewtile);
                }
            }
        }
        
		return true;
	}
	
	public override void _DropData(Vector2 position, Variant data)
	{
		GD.Print("Dropping");
        if(data.Obj is Tile tile)
        {
            this.Texture = tile.Texture;
        } else if (data.Obj is ComponentBase component)
        {
            if(component.IsPlacedInGrid == false)
            {
                CreateNewComponent(component);
            }
            else
            {
                MoveExistingComponent(component);
            }
        }
        
    }

    private void CreateNewComponent(ComponentBase componentBlueprint)
    {
        if (GameManager.Instance.Grid.CanComponentBePlaced(GridX, GridY, componentBlueprint))
        {
            GameManager.Instance.Grid.InstantiateComponentFromBlueprint(GridX, GridY, componentBlueprint);
        }
    }

    private void MoveExistingComponent(ComponentBase component)
    {
        if (GameManager.Instance.Grid.CanComponentBePlaced(GridX, GridY, component))
        {
            GameManager.Instance.Grid.RemoveComponentAt(component.GridXMainTile, component.GridYMainTile);
            GameManager.Instance.Grid.InstantiateComponentFromBlueprint(GridX, GridY, component);
        }
    }

    public override Variant _GetDragData(Vector2 position)
	{
		return this.Component;
	}
    public Tile Duplicate()
    {
        var copy = base.Duplicate() as Tile;
        copy.Pins = new Dictionary<RectangleSide, Pin>();
        if(Pins != null)
        {
            foreach (var kvp in Pins)
            {
                copy.Pins.Add(kvp.Key, (Pin)kvp.Value.Duplicate());
            }
        }
        return copy;
    }
}
