using Godot;
using System;

public partial class PICGrid : GridContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i=0; i < 36 - 1; i++)
		{
			this.AddChild(this.GetNode("PICRect").Duplicate());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
