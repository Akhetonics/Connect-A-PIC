using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class Tile : TextureRect
{
    public ComponentBase Component { get; set; }
    public delegate void TileEventHandler(Tile tile);
    public delegate void ComponentEventHandler(Tile tile, ComponentBase component);
    public event TileEventHandler OnDeletionRequested;
    public event TileEventHandler OnRotationRequested;
    public event ComponentEventHandler OnMoveComponentRequested;
    public event ComponentEventHandler OnCreateNewComponentRequested;
    protected Dictionary<RectangleSide, Pin> Pins;
    public DiscreteRotation Rotation90Based { get; private set; }
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    public override void _Ready()
    {
        Pins = new Dictionary<RectangleSide, Pin>();
        PivotOffset = Size / 2;
    }
    
    /// <summary>
    /// Register the Tile when it gets created
    /// </summary>
    public void SetPositionInGrid (int X, int Y)
    {
        GridX = X;
        GridY = Y;
    }
    public void ResetToDefault(Texture2D baseTexture)
    {
        Pins = new Dictionary<RectangleSide, Pin>();
        Texture = baseTexture;
        Visible = true;
        SetRotation90Based(DiscreteRotation.R0);
        Component = null;
    }
    public void SetRotation90Based(DiscreteRotation rotation90)
    {
        int rotationIntervals = ((int)rotation90 - (int)Rotation90Based) % ((int)DiscreteRotation.R270+1);
        for(int i = 0; i < rotationIntervals; i++)
        {
            RotateBy90();
        }
    }
    public void InitializePins(Pin right, Pin up, Pin left, Pin down)
    {
        Pins = new Dictionary<RectangleSide, Pin>
        {
            { RectangleSide.Right, right },
            { RectangleSide.Up, up },
            { RectangleSide.Left, left },
            { RectangleSide.Down, down }
        };
    }
    public void RotateBy90()
    {
        this.Rotation90Based = (DiscreteRotation)((int)(Rotation90Based + 1) % (int)(DiscreteRotation.R270+1));
        RotationDegrees = (int)Rotation90Based * 90;
        // switch all pins around, so that they all go one to the left as with the rotation
        if (Pins != null && Pins.Count >= 4)
        {
            (Pins[RectangleSide.Right], Pins[RectangleSide.Down]) = (Pins[RectangleSide.Down], Pins[RectangleSide.Right]);
            (Pins[RectangleSide.Right], Pins[RectangleSide.Left]) = (Pins[RectangleSide.Left], Pins[RectangleSide.Right]);
            (Pins[RectangleSide.Right], Pins[RectangleSide.Up]) = (Pins[RectangleSide.Up], Pins[RectangleSide.Right]);
        }
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
            ShowMultiTileDragPreview(position, component);
        }

        return true;
    }
    protected void ShowMultiTileDragPreview(Vector2 position, ComponentBase component)
    {
        var previewGrid = new GridContainer();
        previewGrid.Columns = component.WidthInTiles;
        for (int x = 0; x < component.WidthInTiles; x++)
        {
            for (int y = 0; y < component.HeightInTiles; y++)
            {
                var previewtile = component.GetSubTileAt(x, y).Duplicate() as Tile;
                previewGrid.AddChild(previewtile);
            }
        }
        this.SetDragPreview(previewGrid);
    }
    public override void _DropData(Vector2 position, Variant data)
    {
        if (data.Obj is ComponentBase component)
        {
            if (component.IsPlacedInGrid == false)
            {
                OnCreateNewComponentRequested?.Invoke(this, component);
            }
            else
            {
                OnMoveComponentRequested?.Invoke(this, component);
            }
        }
    }

    public override Variant _GetDragData(Vector2 position)
    {
        return this.Component;
    }
    public Tile Duplicate()
    {
        var copy = base.Duplicate() as Tile;
        copy.Rotation90Based = Rotation90Based;
        copy.RotationDegrees = RotationDegrees;
        copy.Pins = new Dictionary<RectangleSide, Pin>();
        if (Pins != null)
        {
            foreach (var kvp in Pins)
            {
                copy.Pins.Add(kvp.Key, kvp.Value.Duplicate());
            }
        }
        return copy;
    }
    public override void _GuiInput(InputEvent inputEvent)
    {
        base._GuiInput(inputEvent);
        if (inputEvent is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Position.X < 0 || mouseEvent.Position.Y < 0 || mouseEvent.Position.X > Size.X || mouseEvent.Position.Y > Size.Y)
            {
                return;
            }
            if (mouseEvent.ButtonIndex == MouseButton.Middle && mouseEvent.Pressed)
            {
                OnDeletionRequested?.Invoke(this);
            }
            if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
            {
                OnRotationRequested?.Invoke(this);
            }
        }

    }
}
