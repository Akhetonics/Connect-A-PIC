using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;

public partial class TemplateTile : TileDraggable
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

    public override void _DropData(Vector2 position, Variant data)
    { // you cannot drop something on a template tile
    }
	
	public override Variant _GetDragData(Vector2 position)
	{
        return componentTemplate;
	}
}
