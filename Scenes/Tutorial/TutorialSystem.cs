using Godot;
using System;
using System.Collections.Generic;

public partial class TutorialSystem : Control
{
    [Export] Control TutorialPopup { get; set; }
   
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Control ExclusionZoneContainer { get; set; }
    [Export] TextureRect ExclusionCircle { get; set; }
    [Export] TextureRect ExclusionSquare { get; set; }

    [Export] Node2D PortContainer { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Control ToolBoxContainer { get; set; }


    /// <summary>
    /// Describes tutorial scenario, tutorial starts from 0th element to the end
    /// each tutorial state defines tutorial stage
    /// </summary>
    public List<TutorialState> TutorialScenario { get; set; }

    private int currentStateIndex = 0;

    private RichTextLabel Title;
    private RichTextLabel Body;

    private Control YesNoConfiguration;
    private Control QuitSkipNextConfig;
    private Control SkipContainer;
    private Control NextContainer;


    private int portContainerOffset = 124;
    private int portsWidth = 120;
    private int portHeight = 62;

    /// <summary>
    /// Represents height and width of a square tile in grid
    /// </summary>
    private int tileSize = 62;

    /// <summary>
    /// Represents global position of top left corner of the grid
    /// </summary>
    private Vector2 gridGlobalPosition;

    /// <summary>
    /// Represents the view port position of menu buttons (default left top corner)
    /// </summary>
    private Vector2 menuButtonPosition = Vector2.Zero;



    //TODO: needs to be removed after debugging
    int i = 0;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            // Check if the space key is pressed
            if (eventKey.Pressed && eventKey.Keycode == Key.Space)
            {
                //TODO: debugging code remove afterwards
                if (i == 0)
                {
                    ClearExclusionZones();
                    HighlightControlNodeWithCustomSize(PortContainer, customXOffset: -portsWidth, customYOffset: portContainerOffset, customXSize: portsWidth, customYSize: portHeight * 8);
                    i++;
                    SetTutorialPopupCenter();
                    SetYesNoConfiguration();
                }
                else if (i == 1)
                {
                    ClearExclusionZones();
                    HighlightControlNode(MenuBar, allMargins:3f);
                    i++;
                    SetTutorialPopupTopRight();
                    SetQuitNextConfiguration();
                }
                else
                {
                    ClearExclusionZones();
                    HighlightControlNode(ToolBoxContainer);
                    i = 0;
                    SetQuitSkipConfiguration();
                }
            }
        }
    }

    public override void _Ready()
    {
        Visible = false;

        Title = GetNode<RichTextLabel>("%Title");
        Body  = GetNode<RichTextLabel>("%Body");

        YesNoConfiguration = GetNode<Control>("%YesNoConfiguration");
        QuitSkipNextConfig = GetNode<Control>("%QuitSkipNextConfiguration");
        SkipContainer      = GetNode<Control>("%SkipContainer");
        NextContainer      = GetNode<Control>("%NextContainer");

        ExclusionZoneContainer.RemoveChild(ExclusionCircle);
        ExclusionZoneContainer.RemoveChild(ExclusionSquare);

        DarkeningArea.MouseFilter = MouseFilterEnum.Ignore;

        // TODO: check if don't show again was marked
    }

    public override void _Process(double delta)
    {

    }

    private void GoToNextState()
    {
        if (TutorialScenario.Count == 0)
            throw new Exception("Tutorial scenario not defined!");

        if (currentStateIndex == TutorialScenario.Count - 1)
        {
            QuitTutorial();
            return;
        }

        currentStateIndex++;

        var newTutorialState = TutorialScenario[currentStateIndex];

        SetupTutorialFrom(newTutorialState);
    }

    private void QuitTutorial()
    {
        //TODO: check if don't show again is marked and if it is then write in app data so that it won't be shown again
    }

    private void SetupTutorialFrom(TutorialState state)
    {
        Title.Text = state.Title;
        Body.Text = state.Body;

        switch (state.WindowPlacement)
        {
            case WindowPlacement.Center: SetTutorialPopupCenter(); break;
            case WindowPlacement.TopRight: SetTutorialPopupTopRight(); break;
        }

        switch (state.ButtonsArrangement)
        {
            case ButtonsArrangement.YesNo: SetYesNoConfiguration(); break;
            case ButtonsArrangement.QuitSkip: SetQuitSkipConfiguration(); break;
            case ButtonsArrangement.QuitNext: SetQuitNextConfiguration(); break;
        }

        ClearExclusionZones();

        foreach (var higlitedControl in state.HiglitedControls)
        {
            if(higlitedControl.customXSize == 0 && higlitedControl.customYSize == 0)
            {
                HighlightControlNode(higlitedControl.HiglitedNode,
                    higlitedControl.marginTop, higlitedControl.marginRight, higlitedControl.marginBottom, higlitedControl.marginBottom,
                    higlitedControl.XOffset, higlitedControl.YOffset);
            }
            else
            {
                HighlightControlNodeWithCustomSize(higlitedControl.HiglitedNode,
                    higlitedControl.marginTop, higlitedControl.marginRight, higlitedControl.marginBottom, higlitedControl.marginBottom,
                    higlitedControl.XOffset, higlitedControl.YOffset,
                    higlitedControl.customXSize, higlitedControl.customYSize);
            }
        }

        foreach (var higlitedNode in state.HiglitedNodes)
        {
            HighlightControlNodeWithCustomSize(higlitedNode.HiglitedNode,
                higlitedNode.marginTop, higlitedNode.marginRight, higlitedNode.marginBottom, higlitedNode.marginBottom,
                higlitedNode.XOffset, higlitedNode.YOffset,
                higlitedNode.customXSize, higlitedNode.customYSize);
        }
    }


    private void SetTitleText(string text)
    {
        Title.Text = $"[center]{text}[/center]";
    }
    private void SetBodyText(string text)
    {
        Body.Text = $"[center]{text}[/center]";
    }

    private void SetTutorialPopupCenter()
    {
        TutorialPopup.SetAnchorsAndOffsetsPreset(LayoutPreset.Center,LayoutPresetMode.KeepSize);
    }
    private void SetTutorialPopupTopRight()
    {
        TutorialPopup.SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight, LayoutPresetMode.KeepSize);
    }

    private void SetQuitSkipConfiguration()
    {
        YesNoConfiguration.Visible = false;
        NextContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        SkipContainer.Visible = true;

    }
    private void SetQuitNextConfiguration()
    {
        YesNoConfiguration.Visible = false;
        SkipContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        NextContainer.Visible = true;
    }
    private void SetYesNoConfiguration()
    {
        YesNoConfiguration.Visible = true;
        QuitSkipNextConfig.Visible = false;
    }

    private void HighlightControlNode(Control control,
        float allMargins, float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNode(control, allMargins, allMargins, allMargins, allMargins, customXOffset, customYOffset);
    }
    private void HighlightControlNode(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0)
    {
        HighlightControlNodeWithCustomSize(control,
            marginTop, marginRight, marginBotton, marginLeft,
            customXOffset, customYOffset,
            control.Size.X, control.Size.Y);
    }
    private void HighlightControlNodeWithCustomSize(Control control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }
    private void HighlightControlNodeWithCustomSize(Node2D control,
        float marginTop = 0, float marginRight = 0, float marginBotton = 0, float marginLeft = 0,
        float customXOffset = 0, float customYOffset = 0,
        float customXSize = 0, float customYSize = 0)
    {
        TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

        if (exclusionZone == null) return;

        ExclusionZoneContainer.AddChild(exclusionZone);

        Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset,
                control.GlobalPosition.Y - marginTop + customYOffset);

        GetViewport().SizeChanged += () => {
            exclusionZone.GlobalPosition = position;
        };

        exclusionZone.GlobalPosition = position;

        exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBotton);
        exclusionZone.Visible = true;
    }

    private void ClearExclusionZones()
    {
        foreach (var child in ExclusionZoneContainer.GetChildren())
        {
            child.QueueFree();
        }

    }


    private void OnYesButtonPress()
    {
        // TODO: goes to next slide or if end of the line quits
    }
    private void OnNoButtonPress()
    {
        // TODO: quits tutorial
    }
    private void OnQuitButtonPress()
    {
        // TODO: quits tutorial
    }
    private void OnNextButtonPress()
    {
        // TODO: checks completion condition and if competed goes to next one
    }
    private void OnSkipButtonPress()
    {
        // TODO: 
    }

}
