using Godot;
using System;

public partial class PICRect : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		GD.Print("Dropping");
		this.Color = new Color(255, 0, 0);
	}
	
	public override Variant _GetDragData(Vector2 position)
	{
		// TEMP: Remove
		GD.Print("start dragging");
		return this;
	}
}
