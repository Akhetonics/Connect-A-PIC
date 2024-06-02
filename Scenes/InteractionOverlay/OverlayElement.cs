using ConnectAPIC.Scenes.InteractionOverlay;
using Godot;
using System;
using System.ComponentModel;

public partial class OverlayElement : Node2D
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
    /// Defines if mouse is inside any of the Interaction Areas
    /// </summary>
    public bool MouseInsideAreas { get; set; }

    /// <summary>
    /// Invoked if mouse cursor enters any one of the areas
    /// </summary>
    public event EventHandler<OverlayElement> AreaEntered;

    /// <summary>
    /// Invoked when mouse cursor leaves all areas (is in no area from <see cref="InteractionAreas"/>)
    /// </summary>
    public event EventHandler<OverlayElement> AreaExited;

	public override void _Ready()
	{
        foreach (Area2D area in InteractionAreas){
            area.MouseEntered += Area_MouseEntered;
            area.MouseExited += Area_MouseExited;
        }

        this.VisibilityChanged += ActivationStatusChanged;
	}

    private void ActivationStatusChanged()
    {
        if (this.Visible) {
            InteractionOverlayController.Connect(this);
        } else {
            InteractionOverlayController.Disconnect(this);
        }
    }

    private void Area_MouseExited()
    {
        AreaEntered.Invoke(this, this);
        MouseInsideAreas = true;
    }

    private void Area_MouseEntered()
    {
        AreaExited.Invoke(this, this);
        MouseInsideAreas = false;
    }
}
