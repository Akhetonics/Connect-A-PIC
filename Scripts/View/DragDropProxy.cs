using CAP_Core;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ToolBox;
using Godot;
using System;
using System.Collections.Generic;

public partial class DragDropProxy : Control
{
    public delegate Variant GetDragData(Vector2 position);
    public delegate bool CanDropData(Vector2 position, Variant data);
    public delegate void DropData(Vector2 position, Variant data);
    public event EventHandler<InputEvent> InputReceived;
    public GetDragData OnGetDragData { get; set; }
    public CanDropData OnCanDropData { get; set; }
    public DropData OnDropData { get; set; }

    public void Initialize(int widthInTiles, int heightInTiles)
    {
        base._Ready();
        this.Size = new Vector2(widthInTiles * GameManager.TilePixelSize, heightInTiles * GameManager.TilePixelSize);
    }
    public override Variant _GetDragData(Vector2 position)
    {
        if (OnGetDragData == null) return default;
        if (SelectionTool.IsEditSelectionKeyPressed()) return default;
        var data = OnGetDragData(position);
        if (data.Obj is Godot.Collections.Array array && array.Count == 0) return default;
        return data;
    }
    public override bool _CanDropData(Vector2 position, Variant data)
    {
        if (OnCanDropData == null) return default;
        var canDropData = OnCanDropData(position, data);
        return canDropData;
    }
    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (OnDropData == null) return;
        OnDropData(atPosition, data);
    }
    
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        InputReceived?.Invoke(this, @event);
    }
}
