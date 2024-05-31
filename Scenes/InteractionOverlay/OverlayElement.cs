using Godot;
using System;
using System.ComponentModel;

public partial class OverlayElement : Control
{
    /// <summary>
    /// Determines if mouse scrolling is allowed inside the areas
    /// </summary>
    [Export]
    public bool Scrolling { get; set; } = false;

    /// <summary>
    /// Determines if mouse click interaction is allowed inside the areas
    /// </summary>
    [Export]
    public bool Clicking { get; set; } = false;

    /// <summary>
    /// Priority of overlay element (Z-index), higher value rules will be applied
    /// </summary>
    [Export]
    public int OverlayZIndex { get; set; } = 1;

    /// <summary>
    /// List of areas defining interaction area of this component
    /// </summary>
    [Export]
    public Godot.Collections.Array<Area2D> InteractionAreas { get; set; }

    /// <summary>
    /// Invoked if mouse cursor enters any one of the areas
    /// </summary>
    public event EventHandler AreaEntered;

    /// <summary>
    /// Invoked when mouse cursor leaves all areas (is in no area from <see cref="InteractionAreas"/>)
    /// </summary>
    public event EventHandler AreaExited;

	public override void _Ready()
	{
        foreach (Area2D area in InteractionAreas){
            area.MouseEntered += Area_MouseEntered;
            area.MouseExited += Area_MouseExited;
        }
	}

    private void Area_MouseExited()
    {
        AreaEntered.Invoke(this, EventArgs.Empty);
    }

    private void Area_MouseEntered()
    {
        AreaExited.Invoke(this, EventArgs.Empty);
    }
}
