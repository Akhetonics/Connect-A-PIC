using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;

public partial class ToolBox : Node
{
	[Export] public GridContainer gridContainer;
	[Export] public ComponentViewFactory ComponentViewFactory;
	// soll alle ComponentenViews die es gibt hier als Tool anzeigen und automatisch laden. 
	public override void _Ready()
	{
		this.CheckForNull(x => x.gridContainer);
		this.CheckForNull(x => x.ComponentViewFactory);
		//CallDeferred(nameof(SetAvailableTools));
	}

	public void SetAvailableTools()
	{
		var allComponentTypesNRs = ComponentViewFactory.GetAllComponentIDs();
		if(allComponentTypesNRs.Count == 0)
		{
			CallDeferred(nameof(SetAvailableTools));
		}
		foreach (int typeNumber in allComponentTypesNRs)
		{
			var bordersize = gridContainer.GetThemeConstant("h_separation");
			var toolPixelSize = GameManager.TilePixelSize - bordersize;
			var componentInstance = ComponentViewFactory.CreateComponentView(typeNumber);
			componentInstance.CustomMinimumSize = new Vector2 (toolPixelSize, toolPixelSize);
			var componentSizeCorrection = (componentInstance.Size / toolPixelSize);
			var biggestScaleFactor = Math.Max(componentSizeCorrection.X, componentSizeCorrection.Y);
			componentInstance.Scale /= biggestScaleFactor;
			TemplateTileView rect = new();
			rect.componentViewFactory = ComponentViewFactory;
			rect.CustomMinimumSize = new Vector2(toolPixelSize , toolPixelSize);
			rect.AddChild(componentInstance);
			gridContainer.AddChild(rect);
		}
	}
}
