using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using Godot;
using System;

public partial class ToolBox : Node
{
	[Export] public GridContainer gridContainer;
	// soll alle ComponentenViews die es gibt hier als Tool anzeigen und automatisch laden. 
	public override void _Ready()
	{
		CallDeferred(nameof(LoadAllComponentsAsTools));
	}

	public void LoadAllComponentsAsTools()
	{
		var allComponentTypes = ComponentViewFactory.Instance.GetAllComponentIDs();
		foreach (Type componentType in allComponentTypes)
		{
			var bordersize = gridContainer.GetThemeConstant("h_separation");
			var toolPixelSize = GameManager.TilePixelSize - bordersize;
			var componentInstance = ComponentViewFactory.Instance.CreateComponentView(componentType);
			componentInstance.CustomMinimumSize = new Vector2 (toolPixelSize, toolPixelSize);
			var componentSizeCorrection = (componentInstance.Size / toolPixelSize);
			var biggestScaleFactor = Math.Max(componentSizeCorrection.X, componentSizeCorrection.Y);
			componentInstance.Scale /= biggestScaleFactor;
			TemplateTileView rect = new();
			rect.CustomMinimumSize = new Vector2(toolPixelSize , toolPixelSize);
			rect.AddChild(componentInstance);
			gridContainer.AddChild(rect);
		}
	}
}
