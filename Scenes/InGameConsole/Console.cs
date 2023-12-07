using CAP_Contracts.Logger;
using CAP_Core.LightFlow;
using Components.ComponentDraftMapper;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Debuggers;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Console : ScrollContainer
{
	[Export] private Node LoggingParent { get; set; }
	[Export] private RichTextLabel InfoTextTemplate { get; set; }
	[Export] private RichTextLabel ErrorTextTemplate { get; set; }
	private bool visibilityChanged = false;

	public override void _Ready()
	{
		this.CheckForNull(x => x.LoggingParent);
		this.CheckForNull(x => x.InfoTextTemplate);
		this.CheckForNull(x => x.ErrorTextTemplate);
		Hide();
	}

	public void Initialize(ILogger logger)
	{
		logger.LogAdded += (Log obj)=> {
			if (obj.Level > LogLevel.Warn)
			{
				PrintErr(obj.Message);
			}
			else
			{
				PrintInfo(obj.Message);
			}
		};
	}
	public void PrintInfo(string text)
	{
		GD.Print(text);
		Print(text);
	}
	public void PrintErr(string text)
	{
		Print(text,true);
		GD.PrintErr(text);
		Show();
	}
	private void Print(string text, bool isError = false)
	{
		RichTextLabel labelTemplate = InfoTextTemplate;
		if (isError)
		{
			labelTemplate = ErrorTextTemplate;
		}

		var newLine = labelTemplate.Duplicate() as RichTextLabel;
		newLine.Text = text;
		newLine.Visible = true;
		LoggingParent.AddChild(newLine);
		LoggingParent.MoveChild(newLine, 0);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.IsCommandOrControlPressed() && eventKey.Keycode == Key.F1)
			{
				if (visibilityChanged == false)
				{
					visibilityChanged = true;
					Visible = !Visible;
				}
			}
			else
			{
				visibilityChanged = false;
			}

			if (eventKey.IsReleased() && eventKey.Keycode == Key.F2)
			{
				var matrixPrinter = new GridSMatrixPrinter(GameManager.instance.GridViewModel.MatrixAnalyzer);
				Print(matrixPrinter.ToString().Replace('\t', ' '));
			}
		}
	}
}