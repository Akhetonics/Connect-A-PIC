using ConnectAPIC.LayoutWindow.View;
using Godot;
using System;

public partial class ToolBox : Node
{
	[Export] public GridContainer gridContainer;
	// soll alle ComponentenViews die es gibt hier als Tool anzeigen und automatisch laden. 
	public override void _Ready()
	{
		CallDeferred(nameof(LoadAllComponents));
	}

	public void LoadAllComponents()
	{
		var allComponentTypes = ComponentViewFactory.Instance.GetAllComponentTypes();
		foreach (Type componentType in allComponentTypes)
		{
			var componentInstance = ComponentViewFactory.Instance.CreateComponentView(componentType);
			gridContainer.AddChild(componentInstance);
		}
	}
}
