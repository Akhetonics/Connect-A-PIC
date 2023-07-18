using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class Tile : TextureRect
{
    public ComponentBase Component;
    private Dictionary<RectangleSide, Pin> Pins;
    private DiscreteRotation rotation;

    public void InitializePins(Pin right, Pin up, Pin left, Pin down)
    {
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
		GD.Print("Over DropZone");
		var cpb = new ColorPickerButton();
		cpb.Color = new Color(255, 0, 0);
		cpb.Size = new Vector2(50, 50);
		this.SetDragPreview(cpb);
		return true;
	}
	
	public override void _DropData(Vector2 position, Variant data)
	{
        // here you can drop either Tiles that have a component which would then be moved if it fits in here
        // or you can add Components which would be instantiated if they fit in here. 
        // we really need the gameManager to do that properly.
		GD.Print("Dropping");
		this.Texture = (data.Obj as TextureRect).Texture;
    }
	
	public override Variant _GetDragData(Vector2 position)
	{
		// TEMP: Remove
		GD.Print("start dragging");
		return this;
	}
}
