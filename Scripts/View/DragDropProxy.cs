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
    private Control DragPreview { get; set; }

    public void Initialize(int widthInTiles, int heightInTiles)
    {
        base._Ready();
        this.Size = new Vector2(widthInTiles * GameManager.TilePixelSize, heightInTiles * GameManager.TilePixelSize);
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
        DragPreview = null;
        OnDropData(atPosition, data);
    }
    public void ShowMultiTileDragPreview(Godot.Vector2 position, ComponentBaseView component, bool canDropData = true)
    {
        CheckIfDragWasResetted();

        if (DragPreview == null)
        {
            DragPreview = component.Duplicate();
            DragPreview.Visible = false;
            if (canDropData)
            {
                DragPreview.Modulate = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                DragPreview.Modulate = new Color(1, 0, 0, 0.5f);
            }
        }
        else
        {
            DragPreview.Visible = true;
        }

        DragPreview.Position = position + this.GlobalPosition + component.GetPositionDisplacementAfterRotation();
        SetDragPreview(DragPreview);



    }

    private void CheckIfDragWasResetted()
    {
        if (DragPreview == null) return;
        try
        {
            if (DragPreview.IsQueuedForDeletion())
            {
                DragPreview = null;
            }
        }
        catch
        {
            DragPreview = null;
        }
    }
}
