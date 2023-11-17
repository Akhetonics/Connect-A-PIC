using CAP_Core.LightFlow;
using Components.ComponentDraftMapper;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Debuggers;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

public record LogInfo
{
	public string Info;
	public bool IsError;
}
public partial class CustomLogger : ScrollContainer, ILogger
{
	private ScrollContainer console;
	[Export] private Node _LoggingParent { get; set; }
	[Export] private RichTextLabel _InfoText { get; set; }
	[Export] private RichTextLabel _ErrorText { get; set; }
	public Node LoggingParent { get; set; }
	public RichTextLabel InfoTextTemplate { get; set; }
	public RichTextLabel ErrorTextTemplate { get; set; }
	private bool visibilityChanged = false;
	private List<LogInfo> LogInfos = new();
	public static CustomLogger inst { get; set; }
	public override void _Ready()
	{
		if(inst == null)
		{
			inst = this;
		} 
		else
		{
			QueueFree();
			return;
		}
		inst.LoggingParent = _LoggingParent;
        inst.InfoTextTemplate = _InfoText;
        inst.ErrorTextTemplate = _ErrorText;
        inst.console = this;
        inst.Visible = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		FlushBufferedLogs();
		
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

			if(eventKey.IsReleased() && eventKey.Keycode == Key.F2)
			{
				TreePrinter.PrintTree(GetTree().Root, 0);
			}
			if (eventKey.IsReleased() && eventKey.Keycode == Key.F3)
			{
				var matrixPrinter = new GridSMatrixPrinter(GameManager.instance.GridViewModel.MatrixAnalyzer);
				Print(matrixPrinter.ToString().Replace('\t',' '));
			}
		}
	}

	private void FlushBufferedLogs()
	{
		if (LogInfos.Count != 0 && LoggingParent != null)
		{
			foreach (var info in LogInfos)
			{
				if (info.IsError)
				{
					var text = FormatErrorText(info.Info);
					Print(text);
				}
				else
				{
					var text = FormatErrorText(info.Info);
					Print(text, true);
					console?.Show();
				}
			}
			LogInfos = new List<LogInfo> { };
		}
	}

	public void PrintLn(string text, bool printRawText = false)
	{
		if(!printRawText)
		{
			text = FormatErrorText(text);
		}
		GD.Print(text);
		Print(text);
	}
	public void PrintErr(string text)
	{
		text = FormatErrorText(text);
		Print(text,true);
		GD.PrintErr(text);
		console?.Show();
	}
	public void PrintEx(Exception ex)
	{
		var text = FormatErrorText("msg: " + ex.Message + " stck: " + ex.StackTrace + " inner: " + ex.InnerException);
		Print(text, true);
		GD.PrintErr(text);
		console?.Show();
	}
	private void Print(string text, bool isError = false)
	{
		RichTextLabel labelTemplate = InfoTextTemplate;
		if (isError)
		{
			labelTemplate = ErrorTextTemplate;
		}
		if(labelTemplate != null && LoggingParent != null)
		{
			FlushBufferedLogs();
		} else
		{
			LogInfos.Add(new LogInfo() { Info = text, IsError = isError });
			return;
		}
		var newLine = labelTemplate.Duplicate() as RichTextLabel;
		newLine.Text = text;
		newLine.Visible = true;
		LoggingParent.AddChild(newLine);
		LoggingParent.MoveChild(newLine, 0);
	}

	private static string FormatErrorText(string text)
	{
		try
		{
			StackTrace stackTrace = new(true);
			StackFrame frame = stackTrace.GetFrame(2);
			var method = frame.GetMethod();
			var lineNumber = frame.GetFileLineNumber();
			var declaringType = method.DeclaringType.Name;
			var methodName = method.Name;

			return $"{declaringType}.{methodName}:{lineNumber} > {text}";
		} catch (Exception ex)
		{
			return ex.Message + text;
		}
		
	}
}
