using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public record LogInfo
{
	public string Info;
	public bool IsError;
}
public partial class CustomLogger : ScrollContainer
{
	[Export] private Node _LoggingParent { get; set; }
	[Export] private RichTextLabel _InfoText { get; set; }
	[Export] private RichTextLabel _ErrorText { get; set; }
	public static Node LoggingParent { get; set; }
	public static RichTextLabel InfoTextTemplate { get; set; }
	public static RichTextLabel ErrorTextTemplate { get; set; }
	private bool visibilityChanged = false;
	private static List<LogInfo> LogInfos = new();
	public override void _Ready()
	{
		LoggingParent = _LoggingParent;
		InfoTextTemplate = _InfoText;
		ErrorTextTemplate = _ErrorText;
		Visible = false;
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
        }
    }

    private static void FlushBufferedLogs()
    {
        if (LogInfos.Count != 0)
        {
            foreach (var info in LogInfos)
            {
                if (info.IsError)
                {
                    PrintErr(info.Info);
                }
                else
                {
                    PrintLn(info.Info);
                }
            }
            LogInfos = new List<LogInfo> { };
        }
    }

    public static void PrintLn(string text)
	{
        text = FormatErrorText(text);
        Print(text);
	}
	public static void PrintErr(string text)
	{
		text = FormatErrorText(text);
        Print(text,true);
	}
	private static void Print(string text, bool isError = false)
    {
        
        RichTextLabel labelTemplate = InfoTextTemplate;
        if (isError)
        {
            labelTemplate = ErrorTextTemplate;
        }
        if(labelTemplate != null)
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
    }

    private static string FormatErrorText(string text)
    {
        StackTrace stackTrace = new StackTrace(true);
        StackFrame frame = stackTrace.GetFrame(2);
        var method = frame.GetMethod();
        var lineNumber = frame.GetFileLineNumber();
        var declaringType = method.DeclaringType;
        var methodName = method.Name;

        return $"{declaringType}.{methodName}:{lineNumber} >> {text}";
    }
}
