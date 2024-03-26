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
    public GetDragData OnGetDragData;
    public CanDropData OnCanDropData;
    public DropData OnDropData;
    private Control DragPreview { get; set; }

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
        //if(data.As<Control>() != null)
        //{
        //    ShowComponentDragPreview(position, data.As<Control>(), canDropData);
        //}
        return canDropData;
    }
    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (OnDropData == null) return;
        DragPreview = null;
        OnDropData(atPosition, data);
    }
    public void ShowComponentDragPreview(Godot.Vector2 position, Control data, bool canDropData = true)
    {
        if (data == null) return;
        CheckIfDragWasReset();
        if (DragPreview == null && data is ComponentView originalComponent)
        {
            DragPreview = originalComponent.Duplicate();
            DragPreview.Visible = false;
        }
        else
        {
            DragPreview.Visible = true;
        }

        if (canDropData)
        {
            DragPreview.Modulate = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            DragPreview.Modulate = new Color(1, 0, 0, 0.5f);
        }
        DragPreview.Position = position + this.GlobalPosition ;
        SetDragPreview(DragPreview);
    }

    private void CheckIfDragWasReset()
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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        InputReceived?.Invoke(this, @event);
    }
}
