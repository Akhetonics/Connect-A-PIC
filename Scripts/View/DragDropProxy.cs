using CAP_Core;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;

public partial class DragDropProxy : Control
{
    public delegate Variant GetDragData(Vector2 position);
    public delegate bool CanDropData(Vector2 position, Variant data);
    public delegate void DropData(Vector2 position, Variant data);
    public event GetDragData OnGetDragData;
    public event CanDropData OnCanDropData;
    public event DropData OnDropData;

    public void Initialize(int widthInTiles , int heightInTiles)
    {
        base._Ready();
        this.Size = new Vector2(widthInTiles * TileView.TilePixelSize, heightInTiles * TileView.TilePixelSize);
    }
    public override Variant _GetDragData(Vector2 position)
    {
        if (OnGetDragData == null) return default;
        return OnGetDragData(position);
    }
    public override bool _CanDropData(Vector2 position, Variant data)
    {
        if (OnCanDropData == null) return default;
        return OnCanDropData(position, data);
    }
    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (OnDropData == null) return;
        OnDropData(atPosition, data);
    }
}
