using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public enum WindowPlacement
{
    TopRight,
    Center
}

public enum ButtonsArrangement
{
    YesNo,
    QuitSkip,
    QuitNext
}


public class Highlited<T>
{
    public T HiglitedNode;

    public float marginTop = 0;
    public float marginLeft = 0;
    public float marginRight = 0;
    public float marginBottom = 0;

    public float XOffset = 0;
    public float YOffset = 0;

    public float customXSize = 0;
    public float customYSize = 0;
}

public class TutorialState
{
    public WindowPlacement WindowPlacement {  get; set; }
    public ButtonsArrangement ButtonsArrangement {  get; set; } 

    public string Title { get; set; }
    public string Body { get; set; }
    public Func<bool> CompletionCondition { get; set; }

    public List<Highlited<Control>> HiglitedControls { get; set; }
    public List<Highlited<Node2D>> HiglitedNodes { get; set; }

    public TutorialState(
        WindowPlacement windowPlacement,
        ButtonsArrangement buttonsArrangement,
        string title,
        string body,
        Func<bool> completionCondition
    )
    {
        WindowPlacement = windowPlacement;
        ButtonsArrangement = buttonsArrangement;
        Title = title;
        Body = body;
        CompletionCondition = completionCondition;
    }
}
