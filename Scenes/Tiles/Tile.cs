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

    /// <summary>
    /// Register the Tile when it gets created
    /// </summary>
    public void RegisterMainGridXY(int X, int Y)
    {
        GridX = X;
        GridY = Y;
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
        this.rotation =(DiscreteRotation)(((int)(this.rotation + 1) % (int)(DiscreteRotation.R270)));
        if (rotation == DiscreteRotation.R90) RotationDegrees = 90;
        if (rotation == DiscreteRotation.R180) RotationDegrees = 180;
        if (rotation == DiscreteRotation.R270) RotationDegrees = 270;
        // switch all pins around, so that they all go one to the left as with the rotation
        (Pins[RectangleSide.Right], Pins[RectangleSide.Up]) = (Pins[RectangleSide.Up], Pins[RectangleSide.Right]);
        (Pins[RectangleSide.Right], Pins[RectangleSide.Left]) = (Pins[RectangleSide.Left], Pins[RectangleSide.Right]);
        (Pins[RectangleSide.Right], Pins[RectangleSide.Down]) = (Pins[RectangleSide.Down], Pins[RectangleSide.Right]);
    }
    
    public Pin GetPinAt(RectangleSide side) // takes rotation into account
    {
        return Pins.GetValueOrDefault(side);
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Pins = new Dictionary<RectangleSide, Pin>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		// TEMP: Remove
		GD.Print("start dragging");
		return this.Component;
	}
}
