using CAP_Contracts.Logger;
using CAP_Core.Helpers;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using SuperNodes.Types;
using System;
using System.Collections.Generic;

[SuperNode(typeof(Dependent))]
public partial class DragDropProxy : Control
{
    public override partial void _Notification(int what);
    [Dependency] public ToolViewModel ToolViewModel => DependOn<ToolViewModel>();
    public delegate Variant GetDragData(Vector2 position);
    public delegate bool CanDropData(Vector2 position, Variant data);
    public delegate void DropData(Vector2 position, Variant data);
    public event EventHandler<InputEvent> InputReceived;
    public GetDragData OnGetDragData { get; set; }
    public CanDropData OnCanDropData { get; set; }
    public DropData OnDropData { get; set; }
    public GridView GridView { get; private set; }
    public GridViewModel ViewModel { get; private set; }
    private Dictionary<Vector2I, ComponentView> previewComponents = new Dictionary<Vector2I, ComponentView>();
    public ILogger Logger { get; private set; }
    public Vector2I StartGridXY { get; private set; }

    public void Initialize(GridView gridView, GridViewModel viewModel, ILogger logger)
    {
        base._Ready();
        GridView = gridView;
        ViewModel = viewModel;
        Logger = logger;
        Size = new Vector2(ViewModel.Width * GameManager.TilePixelSize, ViewModel.Height * GameManager.TilePixelSize);
    }
    public override bool _CanDropData(Godot.Vector2 position, Variant data)
    {
        try
        {
            bool canDropData = false;
            var deltaGridXY = (GridView.LocalToMap(position) - StartGridXY).ToIntVector();
            var args = ConvertGodotListToMoveComponentArgs(data, deltaGridXY);
            if (args != null)
            {
                canDropData = ViewModel.MoveComponentCommand.CanExecute(args);
            }
            if (canDropData == true && data.Obj is Godot.Collections.Array transitions)
            {
                ShowComponentDragPreview(position, transitions, canDropData);
            }
            if (canDropData == false)
            {
                ClearDragPreview();
            }
            return canDropData;
        }
        catch (Exception ex)
        {
            GridView.Logger.PrintErr(ex.ToString());
            return false;
        }
    }
    public override void _DropData(Godot.Vector2 atPosition, Variant data)
    {
        var deltaGridXY = (GridView.LocalToMap(atPosition) - StartGridXY).ToIntVector();
        var args = ConvertGodotListToMoveComponentArgs(data, deltaGridXY);
        if (args != null)
        {
            ViewModel.MoveComponentCommand.ExecuteAsync(args).Wait();
        }
        ClearDragPreview();
    }

    private static MoveComponentArgs ConvertGodotListToMoveComponentArgs(Variant data, IntVector deltaGridXY)
    {
        if (data.Obj is Godot.Collections.Array componentPositionsVariant)
        {
            List<(IntVector Source, IntVector Target)> translations = new();
            foreach (var componentPositionVariant in componentPositionsVariant)
            {
                if (componentPositionVariant.VariantType != Variant.Type.Vector2I) continue;
                var componentPosition = (Vector2I)componentPositionVariant;
                var source = (componentPosition).ToIntVector();
                translations.Add((source, source + deltaGridXY));
            }
            return new MoveComponentArgs(translations);
        }
        return null;
    }

    public void ShowComponentDragPreview(Godot.Vector2 position, Godot.Collections.Array transitions, bool canDropData)
    {
        var deltaGridXY = (GridView.LocalToMap(position) - StartGridXY);
        Color previewColor = canDropData ? new Color(0.5f, 1, 0.5f) : new Color(1, 0, 0); // Light green for valid, red for invalid.
        foreach (Variant componentPositionVariant in transitions)
        {
            if (componentPositionVariant.VariantType != Variant.Type.Vector2I)
            {
                continue;
            }
            var componentGridPosition = (Vector2I)componentPositionVariant;
            var targetGridPosition = componentGridPosition + deltaGridXY;

            // Assuming CreatePreviewComponent is a method that creates a visual representation of the component.
            CreateOrUpdatePreviewComponent(componentGridPosition, targetGridPosition, previewColor);
        }
    }

    public void CreateOrUpdatePreviewComponent(Vector2I originalGridPosition, Vector2I targetGridPosition, Color color)
    {
        if (previewComponents.TryGetValue(originalGridPosition, out var previewComponent))
        {
            // Preview component already exists, update its position and color
            previewComponent.Position = MapToLocalCorrected(targetGridPosition); // Assuming a method to convert grid position to local position
        }
        else
        {
            // Create a new preview component
            var componentToMove= GridView.GridComponentViews[originalGridPosition.X, originalGridPosition.Y];
            if (componentToMove == null)
                return;
            componentToMove.Position = MapToLocalCorrected(targetGridPosition);
            MoveChildToTop(componentToMove);
            previewComponents.Add(originalGridPosition, componentToMove);
        }
    }
    public static void MoveChildToTop( Node childNode)
    {
        var parentNode = childNode.GetParent();
        int newPosition = parentNode.GetChildCount();
        parentNode.MoveChild(childNode, newPosition);
    }

    private Godot.Vector2 MapToLocalCorrected(Vector2I targetGridPosition)
    {
        var tileSize = new Godot.Vector2(GameManager.TilePixelSize, GameManager.TilePixelSize);
        return GridView.MapToLocal(targetGridPosition) - 0.5f * tileSize;
    }

    public void ClearDragPreview()
    {
        foreach (var entry in previewComponents)
        {
            entry.Value.Position = MapToLocalCorrected(entry.Key);
        }
        previewComponents.Clear();
    }

    public override Variant _GetDragData(Godot.Vector2 position)
    {
        if (SelectionTool.IsEditSelectionKeyPressed()) return default;
        if (ToolViewModel.CurrentTool.GetType() != typeof(SelectionTool)) return default; // drag drop only works with the selection tool
        StartGridXY = GridView.LocalToMap(position);
        var selectionMgr = ViewModel.SelectionGroupManager.SelectionManager;
        var selections = selectionMgr.Selections;
        var cursorComponent = GridView.GridComponentViews[StartGridXY.X, StartGridXY.Y];
        var componentLocations = new Godot.Collections.Array<Vector2I>();
        if (cursorComponent != null)
        {
            var cursorComponentPos = new Vector2I(cursorComponent.ViewModel.GridX, cursorComponent.ViewModel.GridY);
            var isComponentInSelection = selectionMgr.Selections.Contains(cursorComponentPos.ToIntVector());
            if (isComponentInSelection)
            {
                foreach (var selection in selections)
                {
                    componentLocations.Add(new Vector2I(selection.X, selection.Y));
                }
            }
            else
            {
                componentLocations.Add(cursorComponentPos);
            }
        }

        if(componentLocations.Count == 0)
        {
            return default;
        } else
        {
            return componentLocations;
        }
    }
    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                if (eventMouseButton.Pressed == false)
                {
                    ClearDragPreview();
                }
            }
        }
    }
}
