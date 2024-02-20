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
		var canDropData = OnCanDropData(position, data);
		if(data.As<Control>() != null)
		{
			ShowComponentDragPreview(position, data.As<Control>(), canDropData);
		}
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
}
