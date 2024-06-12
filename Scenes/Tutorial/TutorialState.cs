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

    /// <summary>
    /// Function defining condition for transition when next is pressed
    /// (so if condition is true tutorial will progress to next state)
    /// </summary>
    public Func<bool> CompletionCondition { get; set; }

    /// <summary>
    /// Function which runs when loading tutorial state
    /// </summary>
    public Action FunctionWhenLoading { get; set; }

    /// <summary>
    /// Function which runs when unloading tutorial state
    /// </summary>
    public Action FunctionWhenUnloading { get; set; }

    public List<Highlited<Control>> HiglitedControls { get; set; } = new();
    public List<Highlited<Node2D>> HiglitedNodes { get; set; } = new();

    public TutorialState(
        WindowPlacement windowPlacement,
        ButtonsArrangement buttonsArrangement,
        string title,
        string body,
        Func<bool> completionCondition
    )
    {
        BasicSetup(windowPlacement, buttonsArrangement, title, body, completionCondition);
    }

    public TutorialState(
        WindowPlacement windowPlacement,
        ButtonsArrangement buttonsArrangement,
        string title,
        string body,
        Func<bool> completionCondition,
        List<Highlited<Control>> higlitedControls
        )
    {
        BasicSetup(windowPlacement, buttonsArrangement, title, body, completionCondition);
        HiglitedControls = higlitedControls;
    }

    public TutorialState(
    WindowPlacement windowPlacement,
    ButtonsArrangement buttonsArrangement,
    string title,
    string body,
    Func<bool> completionCondition,
    List<Highlited<Node2D>> higlitedNodes
    )
    {

        BasicSetup(windowPlacement, buttonsArrangement, title, body, completionCondition);
        HiglitedNodes = higlitedNodes;
    }

    public TutorialState(
    WindowPlacement windowPlacement,
    ButtonsArrangement buttonsArrangement,
        string title,
        string body,
        Func<bool> completionCondition,
        List<Highlited<Control>> higlitedControls,
        List<Highlited<Node2D>> higlitedNodes
        )
    {
        BasicSetup(windowPlacement, buttonsArrangement, title, body, completionCondition);
        HiglitedControls = higlitedControls;
        HiglitedNodes = higlitedNodes;
    }



    public void RunSetupFunction()
    {
        if (FunctionWhenLoading != null)
            FunctionWhenLoading.Invoke();
    }

    public void RunUnloadFunction()
    {
        if (FunctionWhenUnloading != null)
            FunctionWhenUnloading.Invoke();
    }


    private void BasicSetup(WindowPlacement windowPlacement, ButtonsArrangement buttonsArrangement, string title, string body, Func<bool> completionCondition)
    {
        WindowPlacement = windowPlacement;
        ButtonsArrangement = buttonsArrangement;
        Title = $"[center]{title}[/center]";
        Body = $"[center]{body}[/center]";
        CompletionCondition = completionCondition;
    }

}
