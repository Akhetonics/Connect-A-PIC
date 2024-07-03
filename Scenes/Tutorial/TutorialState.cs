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

public enum ButtonsConfiguration
{
    YesNo,
    QuitSkip,
    QuitNext,
    Finish
}


public class TutorialState
{
    public WindowPlacement WindowPlacement {  get; set; }
    public ButtonsConfiguration ButtonsConfiguration {  get; set; } 

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

    public List<HighlightedElement<Control>> HighlightedControls { get; set; } = new();
    public List<HighlightedElement<Node2D>> HighlightedNodes { get; set; } = new();

    public TutorialState(
        WindowPlacement windowPlacement,
        ButtonsConfiguration buttonsArrangement,
        string title,
        string body,
        Func<bool> completionCondition,
        List<HighlightedElement<Control>> HighlightedControls = null,
        List<HighlightedElement<Node2D>> HighlightedNodes = null
    )
    {
        BasicSetup(windowPlacement, buttonsArrangement, title, body, completionCondition);

        if ( HighlightedControls != null ) 
            this.HighlightedControls = HighlightedControls;

        if ( HighlightedNodes != null )
            this.HighlightedNodes = HighlightedNodes;
    }


    public bool AddHighlightedElemenet<T>(HighlightedElement<T> highlightedElement)
    {
        if (typeof(T) == typeof(Node2D) && highlightedElement as HighlightedElement<Node2D> != null )
        {
            if (HighlightedNodes == null)
                HighlightedNodes = new();
            
            HighlightedNodes.Add(highlightedElement as HighlightedElement<Node2D>);
        }
        else if (typeof(T) == typeof(Control) && highlightedElement as HighlightedElement<Control> != null)
        {
            if (HighlightedControls == null)
                HighlightedControls = new();

            HighlightedControls.Add(highlightedElement as HighlightedElement<Control>);
        }
        else
        {
            return false;
        }

        return true;
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

    private void BasicSetup(WindowPlacement windowPlacement, ButtonsConfiguration buttonsArrangement, string title, string body, Func<bool> completionCondition)
    {
        WindowPlacement = windowPlacement;
        ButtonsConfiguration = buttonsArrangement;
        Title = $"[center]{title}[/center]";
        Body = $"[center]{body}[/center]";
        CompletionCondition = completionCondition;
    }

}
