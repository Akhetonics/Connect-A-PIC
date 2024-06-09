using Godot;
using System;

public partial class TutorialSystem : Control
{
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Control ExclusionZoneContainer { get; set; }
    [Export] TextureRect ExclusionCircle { get; set; }
    [Export] TextureRect ExclusionSquare { get; set; }

    [Export] Node2D PortContainer { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Control ToolBoxContainer { get; set; }

    private int portContainerOffset = 124;
    private int portsWidth = 120;
    private int portHeight = 62;

    /// <summary>
    /// Represents height and width of a square tile in grid
    /// </summary>
    private int tileSize = 62;

    /// <summary>
    /// Represents global position of top left corner of the grid
    /// </summary>
    private Vector2 gridGlobalPosition;

    /// <summary>
    /// Represents the view port position of menu buttons (default left top corner)
    /// </summary>
    private Vector2 menuButtonPosition = Vector2.Zero;

    int i = 0;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            // Check if the space key is pressed
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                //TODO: debugging code remove afterwards
                if (i == 0)
                {
                    ClearExclusionZones();
                    HighlightControlNodeWithCustomSize(PortContainer, customXOffset: -portsWidth, customYOffset: portContainerOffset, customXSize: portsWidth, customYSize: portHeight * 8);
                    i++;
                }
                else if (i == 1)
                {
                    ClearExclusionZones();
                    HighlightControlNode(MenuBar, allMargins:3f);
                    i++;
                }
                else
                {
                    ClearExclusionZones();
                    HighlightControlNode(ToolBoxContainer);
                    i = 0;
                }
            }
        }
    }

    public override void _Ready()
    {
        ExclusionZoneContainer.RemoveChild(ExclusionCircle);
        ExclusionZoneContainer.RemoveChild(ExclusionSquare);
    }

    public override void _Process(double delta)
    {

    }

    private void HighlightControlNode(Control control,
        float allMargins, float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNode(control, allMargins, allMargins, allMargins, allMargins, customXOffset, customYOffset);
    }

    private void HighlightControlNode(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNodeWithCustomSize(control,
            marginTop, marginRight, marginBotton, marginLeft,
            customXOffset, customYOffset,
            control.Size.X, control.Size.Y);
    }

    private void HighlightControlNodeWithCustomSize(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }

    private void HighlightControlNodeWithCustomSize(Node2D control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }

    private void ClearExclusionZones()
    {
        foreach (var child in ExclusionZoneContainer.GetChildren())
        {
            child.QueueFree();
        }

    }
}
