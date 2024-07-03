using Godot;
using System;
using System.Collections.Generic;


public partial class ExclusionControl : Node2D
{
    [Export] MainCamera Camera { get; set; }
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Control ExclusionZoneContainer { get; set; }
    [Export] TextureRect ExclusionCircle { get; set; }
    [Export] TextureRect ExclusionSquare { get; set; }

    public override void _Ready()
    {

        ExclusionZoneContainer.RemoveChild(ExclusionCircle);
        ExclusionZoneContainer.RemoveChild(ExclusionSquare);

        DarkeningArea.MouseFilter = MouseFilterEnum.Stop;
    }

    public void SetCustomHighlight(Vector2 size, Vector2 initialPosition, Func<Vector2> recalculatePosition)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        Vector2 position = initialPosition;

        GetViewport().SizeChanged += () => {
            Vector2 newPosition = recalculatePosition.Invoke();
            exclusionZone.Position = newPosition;
        };

        ExclusionZoneContainer.AddChild(exclusionZone);

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = size;
        exclusionZone.Visible = true;
    }

    public void SetCustomHighlight<T>(HighlightedElement<T> highlighted, Func<Vector2> getPositionWithNoOffsets)
    {
        float marginTop = highlighted.marginTop;
        float marginRight = highlighted.marginRight;
        float marginBottom = highlighted.marginBottom;
        float marginLeft = highlighted.marginBottom;
        float customXOffset = highlighted.OffsetX;
        float customYOffset = highlighted.OffsetY;
        float customXSize = highlighted.customSizeX;
        float customYSize = highlighted.customSizeY;

        // control has size and if no custom size is set we use size of control
        if (typeof(T) == typeof(Control) && (highlighted.customSizeX == 0 || highlighted.customSizeY == 0))
        {
            customXSize = (highlighted.HighlightedNode as Control).Size.X;
            customYSize = (highlighted.HighlightedNode as Control).Size.Y;
        }

        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        GetViewport().SizeChanged += () =>
        {
            ReadjustPosition(exclusionZone, getPositionWithNoOffsets, marginTop, marginLeft, customXOffset, customYOffset);
        };

        ReadjustPosition(exclusionZone, getPositionWithNoOffsets, marginTop, marginLeft, customXOffset, customYOffset);

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
        exclusionZone.Visible = true;
    }

    public void SetCustomHighlight(HighlightedElement<Node2D> highlighted)
    {
        float marginTop = highlighted.marginTop;
        float marginRight = highlighted.marginRight;
        float marginBottom = highlighted.marginBottom;
        float marginLeft = highlighted.marginBottom;
        float customXOffset = highlighted.OffsetX;
        float customYOffset = highlighted.OffsetY;
        float customXSize = highlighted.customSizeX;
        float customYSize = highlighted.customSizeY;

        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        GetViewport().SizeChanged += () =>
        {
            ReadjustPosition(exclusionZone,
                () => highlighted.HighlightedNode.Position - new Vector2(Camera.Position.X, Camera.Position.Y),
                marginTop, marginLeft, customXOffset, customYOffset);
        };

        ReadjustPosition(exclusionZone,
            () => highlighted.HighlightedNode.Position - new Vector2(Camera.Position.X, Camera.Position.Y),
            marginTop, marginLeft, customXOffset, customYOffset);

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
        exclusionZone.Visible = true;
    }

    public void SetCustomHighlight(HighlightedElement<Control> highlighted)
    {
        float marginTop = highlighted.marginTop;
        float marginRight = highlighted.marginRight;
        float marginBottom = highlighted.marginBottom;
        float marginLeft = highlighted.marginBottom;
        float customXOffset = highlighted.OffsetX;
        float customYOffset = highlighted.OffsetY;
        float customXSize = highlighted.customSizeX;
        float customYSize = highlighted.customSizeY;

        //control has size and if no custom size is set we use size of control
        if (highlighted.customSizeX == 0 || highlighted.customSizeY == 0)
        {
            customXSize = highlighted.HighlightedNode.Size.X;
            customYSize = highlighted.HighlightedNode.Size.Y;
        }

        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        GetViewport().SizeChanged += () =>
        {
            ReadjustPosition(exclusionZone,
                () => highlighted.HighlightedNode.Position - new Vector2(Camera.Position.X, Camera.Position.Y),
                marginTop, marginLeft, customXOffset, customYOffset);
        };

        ReadjustPosition(exclusionZone,
            () => highlighted.HighlightedNode.Position - new Vector2(Camera.Position.X, Camera.Position.Y),
            marginTop, marginLeft, customXOffset, customYOffset);

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
        exclusionZone.Visible = true;
    }


    private static void ReadjustPosition(TextureRect exclusionZone, Func<Vector2> getPositionWithNoOffsets, float marginTop, float marginLeft, float customXOffset, float customYOffset)
    {
        Vector2 position = getPositionWithNoOffsets.Invoke();
        position.X += -marginLeft + customXOffset;
        position.Y += -marginTop + customYOffset;
        exclusionZone.GlobalPosition = position;
    }

    public void HighlightControlNode(Control control,
            float allMargins,
            float customXOffset = 0, float customYOffset = 0,
            float customXSize = 0, float customYSize = 0)
    {
        HighlightControlNode(control, allMargins, allMargins, allMargins, allMargins, customXOffset, customYOffset, customXSize, customYSize);
    }

    public void HighlightControlNode(Control control,
        float marginTop, float marginRight, float marginBottom, float marginLeft,
        float customXOffset, float customYOffset,
        float customXSize, float customYSize)
    {
        if (customXSize == 0) customXSize = control.Size.X;
        if (customYSize == 0) customYSize = control.Size.Y;

        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);

        GetViewport().SizeChanged += () => {
            Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
        exclusionZone.Visible = true;
    }
    public void HighlightControlNode(Node2D control,
        float marginTop, float marginRight, float marginBottom, float marginLeft,
        float customXOffset, float customYOffset,
        float customXSize, float customYSize)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);

        GetViewport().SizeChanged += () => {
            Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
        exclusionZone.Visible = true;
    }

    public void ClearExclusionZones()
    {
        foreach (var child in ExclusionZoneContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

}
