using CAP_Contracts.Logger;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;

public partial class ToolBox : Node
{
	[Export] public GridContainer gridContainer;
	// soll alle ComponentenViews die es gibt hier als Tool anzeigen und automatisch laden. 
	public override void _Ready()
	{
		this.CheckForNull(x => x.gridContainer);
	}

	public void SetAvailableTools(ComponentViewFactory ComponentViewFactory, ILogger logger)
	{
		if (ComponentViewFactory == null)
		{
			logger.PrintErr("ComponentViewFactory cannot be null");
			return;
		}
		var allComponentTypesNRs = ComponentViewFactory.GetAllComponentIDs();
		foreach (int typeNumber in allComponentTypesNRs)
		{
			var bordersize = gridContainer.GetThemeConstant("h_separation");
			var toolPixelSize = GameManager.TilePixelSize - bordersize;
			var componentInstance = ComponentViewFactory.CreateComponentView(typeNumber);
			componentInstance.CustomMinimumSize = new Vector2 (toolPixelSize, toolPixelSize);
			var componentSizeCorrection = componentInstance.GetBiggestSize() / toolPixelSize;
			var biggestScaleFactor = Math.Max(componentSizeCorrection.X, componentSizeCorrection.Y);
			if(biggestScaleFactor <= 0)
			{
				logger.PrintErr("biggestScaleFactor is too small, the toolbox cannot scale this component properly of Component NR: " + typeNumber);
			}
			componentInstance.Scale /= biggestScaleFactor;
			TemplateTileView rect = new();
			rect.componentViewFactory = ComponentViewFactory;
			rect.CustomMinimumSize = new Vector2(toolPixelSize , toolPixelSize);
			rect.AddChild(componentInstance);
			gridContainer.AddChild(rect);
		}
	}
}
