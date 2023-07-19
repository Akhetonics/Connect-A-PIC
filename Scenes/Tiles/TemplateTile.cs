using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class TemplateTile : Tile
{
    [Export] public NodePath componentTemplatePath;
    public ComponentBase componentTemplate;
    public override void _Ready()
    {
        base._Ready();
        if (string.IsNullOrEmpty(componentTemplatePath)) throw new ArgumentNullException(nameof(componentTemplatePath));
        Node node = GetNodeOrNull(componentTemplatePath); // not sure what is wrong here but it cannot convert the node to straightline.
        componentTemplate =  (StraightWaveGuide)node;
        if (componentTemplate == null) throw new ArgumentNullException(nameof(componentTemplate));
    }
    public override bool _CanDropData(Vector2 position, Variant data)
	{
        if (data.Obj is ComponentBase component)
        {

            for (int x = 0; x < component.WidthInTiles; x++)
            {
                for (int y = 0; y < component.HeightInTiles; y++)
                {
                    var previewtile = component.GetSubTileAt(x, y).Duplicate() as Tile;
                    previewtile.Position = new Vector2(position.X + x * 64, position.Y + y * 64);
                    
                }
            }
            SetDragPreview(component.GetSubTileAt(0, 0).Duplicate() as Tile);
        }
        return true;
	}
	
	public override void _DropData(Vector2 position, Variant data)
    { // you cannot drop something on a template tile
    }
	
	public override Variant _GetDragData(Vector2 position)
	{
        return componentTemplate;
	}
}
