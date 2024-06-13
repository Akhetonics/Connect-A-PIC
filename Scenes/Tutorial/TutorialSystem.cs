using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scenes.InteractionOverlay;
using ConnectAPIC.Scenes.RightClickMenu;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class TutorialSystem : Control
{
    [Export] Control TutorialPopup { get; set; }
    [Export] Button DoNotShowAgainCheckButton {  get; set; }
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Control ExclusionZoneContainer { get; set; }
    [Export] TextureRect ExclusionCircle { get; set; }
    [Export] TextureRect ExclusionSquare { get; set; }

    [Export] MainCamera Camera { get; set; }
    [Export] Node2D PortContainer { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Control ToolBoxContainer { get; set; }


    /// <summary>
    /// Describes tutorial scenario, tutorial starts from 0th element to the end
    /// each tutorial state defines tutorial stage
    /// </summary>
    public List<TutorialState> TutorialScenario { get; set; } = new List<TutorialState>();

    private int currentStateIndex = -1;

    private RichTextLabel Title;
    private RichTextLabel Body;

    private Control YesNoConfiguration;
    private Control QuitSkipNextConfig;
    private Control FinishConfig;
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

    //TODO: need to move this somewhere where update manager can also access it
    private static string RepoOwnerName = "Akhetonics";
    private static string RepoName = "Connect-A-PIC";

    /// <summary>
    /// Used to determine if tutorial needs to be shown on startup again
    /// when file with this name is present in appdata folder of user then tutorial shouldn't be shown again
    /// </summary>
    string doNotShowAgainMark = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), RepoOwnerName, RepoName, "doNotShowTutorial");


    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            GoToNextState();
        }
        else if (Input.IsActionJustPressed("ui_cancel"))
        {
            QuitTutorial();
        }
    }

    public override void _Ready()
    {
        Visible = false;

        Title = GetNode<RichTextLabel>("%Title");
        Body  = GetNode<RichTextLabel>("%Body");

        YesNoConfiguration = GetNode<Control>("%YesNoConfiguration");
        QuitSkipNextConfig = GetNode<Control>("%QuitSkipNextConfiguration");
        FinishConfig       = GetNode<Control>("%FinishConfiguration");
        SkipContainer      = GetNode<Control>("%SkipContainer");
        NextContainer      = GetNode<Control>("%NextContainer");

        ExclusionZoneContainer.RemoveChild(ExclusionCircle);
        ExclusionZoneContainer.RemoveChild(ExclusionSquare);

        DarkeningArea.MouseFilter = MouseFilterEnum.Stop;

        if (DoNotShowAgainWasChecked()) return;

        SetupSampleTutorial();
        Visible = true;
    }


    public void StartTutorial()
    {
        TutorialScenario.Clear();
        currentStateIndex = -1;
        Camera.RecenterCamera();
        SetupSampleTutorial();
        Visible = true;
    }

    private void SetupSampleTutorial()
    {
        var welcome = new TutorialState(
            WindowPlacement.Center,
            ButtonsArrangement.YesNo,
            "Welcome to Connect-A-PIC",
            "Would you like to go throgh a tutorial?",
            () => true
            );

        welcome.FunctionWhenLoading = () =>
        {
            DoNotShowAgainCheckButton.ButtonPressed = true;
            Camera.autoCenterWhenResizing = true;
            Camera.noZoomingOrMoving = true;
            ExclusionZoneContainer.MouseFilter = MouseFilterEnum.Stop;
        };


        TutorialScenario.Add(welcome);


        var explanation = new TutorialState(
            WindowPlacement.Center,
            ButtonsArrangement.QuitNext,
            "What is Connect-A-PIC",
            "Connect-A-PIC (photonic integrated circuits) aims to simplify the design of optical circuits on a chip",
            () => true
            );

        TutorialScenario.Add(explanation);

        var workingArea = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Main Circuit",
            "This is your main workspace, where you can design and build your own photonic circuits",
            () => true
            );

        workingArea.HiglitedNodes.Add(new Higlighted<Node2D>
        {
            HiglitedNode = PortContainer,
            XOffset = 2,
            customXSize = 1485,
            customYSize = 743
        });

        workingArea.FunctionWhenLoading = () =>
        { 
            (ToolBoxContainer as ToolBoxCollapseControl)?.SetToolBoxToggleState(true);
        };

        workingArea.FunctionWhenUnloading = () =>
        {
            (ToolBoxContainer as ToolBoxCollapseControl)?.SetToolBoxToggleState(false);
        };


        TutorialScenario.Add(workingArea);

        #region ports explanation

        var InputOutputs = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Input Output Ports",
            "Both sides of the main board have input/output ports, this is where you get and read the photonic signal" +
            "\n[color=FFD700]You can left click ports to open control menu where you can change their properties[/color]",
            () => true
            );

        InputOutputs.FunctionWhenLoading = () =>
        {
            HighlightLeftPorts();
        };

        TutorialScenario.Add(InputOutputs);

        #region this is scrapped for now
        //var ChangingPorts = new TutorialState(
        //    WindowPlacement.TopRight,
        //    ButtonsArrangement.QuitNext,
        //    "Port Changing",
        //    "You can customize ports to suit your needs by simply clicking on them and selecting the desired configuration\n" +
        //    "[color=FFD700]Left click first port and change its type to output[/color]",
        //    () => !PortContainer.GetChild<ExternalPortView>(0).ViewModel.IsInput
        //    );

        //ChangingPorts.HiglitedNodes.Add(new Highlighted<Node2D>
        //{
        //    HiglitedNode  = PortContainer,
        //    XOffset       = -portsWidth,
        //    YOffset       = portContainerOffset,
        //    customXSize   = portsWidth,
        //    customYSize   = portHeight
        //});

        //ChangingPorts.FunctionWhenLoading = () =>
        //{
        //    ExclusionZoneContainer.MouseFilter = MouseFilterEnum.Ignore;
        //    DarkeningArea.MouseFilter = MouseFilterEnum.Stop;

        //    ControlMenu = PortContainer.FindChild("ControlMenu", true, false) as ControlMenu;

        //    ControlMenu.VisibilityChanged += () => {
        //        if (ControlMenu.Visible)
        //        {
        //            HighlightControlNode(ControlMenu.GetChild(0) as Control, marginTop: 10, marginRight: 20, marginBotton: 20, marginLeft: 10);
        //        }
        //    };
        //};

        //TutorialScenario.Add(ChangingPorts);
        #endregion

        var InputPorts = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Input Ports",
            "The [color=FFD700]Input Ports[/color] provide you with the photonic signal, think of them as power sources",
            () => true
            );

        InputPorts.HiglitedNodes.Add(new Higlighted<Node2D>
        {
            HiglitedNode = PortContainer,
            XOffset      = -portsWidth,
            YOffset      = portContainerOffset,
            customXSize  = portsWidth,
            customYSize  = portHeight*3
        });

        TutorialScenario.Add(InputPorts);


        var OutputPorts = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Output Ports",
            "The [color=FFD700]Output Ports[/color] give you the ability to read signal strength and phase shift, think of them as power meters",
            () => true
            );

        OutputPorts.HiglitedNodes.Add(new Higlighted<Node2D>
        {
            HiglitedNode = PortContainer,
            XOffset = -portsWidth,
            YOffset = portContainerOffset + portHeight * 3,
            customXSize = portsWidth,
            customYSize = portHeight * 5
        });

        TutorialScenario.Add(OutputPorts);

        #endregion

        #region tool box explanation

        var ToolBox = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Tool Box",
            "Toolbox offers a wide range of components, you can left-click to select component, and left-click (or left-click and drag) to place it on the working grid",
            () => true
            );

        ToolBox.FunctionWhenLoading = () =>
        {
            TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

            if (exclusionZone == null) return;

            Vector2 position = new Vector2(GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X, GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y);

            GetViewport().SizeChanged += () => {
                Vector2 position = new Vector2(GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X, GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y);
                exclusionZone.GlobalPosition = position;
            };
            ExclusionZoneContainer.AddChild(exclusionZone);

            exclusionZone.GlobalPosition = position;

            exclusionZone.Size = new Vector2(ToolBoxContainer.Size.X, ToolBoxContainer.Size.Y);
            exclusionZone.Visible = true;
        };

        TutorialScenario.Add(ToolBox);

        #endregion

        #region menu bar explanation

        var Menu = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsArrangement.QuitNext,
            "Menu Bar",
            "From the menu bar you can export/import your circuit, turn on/off your circuit, export your circuit to NAZCA and undo/redo your actions, also updates will appear here if available",
            () => true
            );

        Menu.FunctionWhenLoading = () =>
        {
            TextureRect exclusionZone = ExclusionSquare.Duplicate() as TextureRect;

            if (exclusionZone == null) return;

            Vector2 position = new Vector2(Camera.Offset.X - 3, Camera.Offset.Y - 3);
                  
            GetViewport().SizeChanged += () => {
                Vector2 position = new Vector2(Camera.Offset.X - 3, Camera.Offset.Y - 3);
                exclusionZone.GlobalPosition = position;
            };

            ExclusionZoneContainer.AddChild(exclusionZone);

            exclusionZone.GlobalPosition = position;

            exclusionZone.Size = new Vector2(MenuBar.Size.X + 6, MenuBar.Size.Y + 6);
            exclusionZone.Visible = true;
        };

        TutorialScenario.Add(Menu);

        #endregion

        var Finished = new TutorialState(
            WindowPlacement.Center,
            ButtonsArrangement.Finish,
            "Tutorial Completed!",
            "Congratulations you've completed tutorial!\n" +
            "[color=FFD700]to open cheatsheet for controls press \"?\" on menu bar[/color]",
            () => true
            );


        TutorialScenario.Add(Finished);

        //TODO: last function should release clicking and scrolling I guess
        Finished.FunctionWhenUnloading = () =>
        {
            Camera.autoCenterWhenResizing = false;
            Camera.noZoomingOrMoving = false;
        };

        GoToNextState();
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

        if (currentStateIndex > 0)
        {
            var currentState = TutorialScenario[currentStateIndex];
            currentState.RunUnloadFunction();
        }

        currentStateIndex++;

        var newTutorialState = TutorialScenario[currentStateIndex];

        SetupTutorialFrom(newTutorialState);
    }

    private void QuitTutorial()
    {
        var currentState = TutorialScenario[currentStateIndex];
        currentState.RunUnloadFunction();

        Camera.autoCenterWhenResizing = false;
        Camera.noZoomingOrMoving = false;

        currentStateIndex = -1;

        if (DoNotShowAgainCheckButton.ButtonPressed)
        {
            SaveDoNotShowAgain();
        }

        this.Visible = false;
    }

    private void SetupTutorialFrom(TutorialState state)
    {
        Title.Text = state.Title;
        Body.Text = state.Body;

        switch (state.WindowPlacement)
        {
            case WindowPlacement.Center:   SetTutorialPopupCenter();   break;
            case WindowPlacement.TopRight: SetTutorialPopupTopRight(); break;

        }

        switch (state.ButtonsArrangement)
        {
            case ButtonsArrangement.YesNo:    SetYesNoConfiguration();    break;
            case ButtonsArrangement.QuitSkip: SetQuitSkipConfiguration(); break;
            case ButtonsArrangement.QuitNext: SetQuitNextConfiguration(); break;
            case ButtonsArrangement.Finish:   SetFinishConfiguration();   break;
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

        state.RunSetupFunction();
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
        FinishConfig.Visible = false;
        YesNoConfiguration.Visible = false;
        NextContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        SkipContainer.Visible = true;

    }
    private void SetQuitNextConfiguration()
    {
        FinishConfig.Visible = false;
        YesNoConfiguration.Visible = false;
        SkipContainer.Visible = false;

        QuitSkipNextConfig.Visible = true;
        NextContainer.Visible = true;
    }
    private void SetYesNoConfiguration()
    {
        YesNoConfiguration.Visible = true;
        QuitSkipNextConfig.Visible = false;
        FinishConfig.Visible = false;
    }

    private void SetFinishConfiguration()
    {
        YesNoConfiguration.Visible = false;
        QuitSkipNextConfig.Visible = false;
        FinishConfig.Visible = true;
    }

    #region Higlight control

    private void HighlightGrid()
    {
        HighlightControlNodeWithCustomSize(PortContainer, customXSize: 1500, customYSize: 743);
    }

    private void HighlightLeftPorts()
    {
        HighlightControlNodeWithCustomSize(PortContainer, customXOffset: -portsWidth, customYOffset: portContainerOffset, customXSize: portsWidth, customYSize: portHeight * 8);
    }

    private void HighlightMenu()
    {
        HighlightControlNode(MenuBar, allMargins: 3f);
    }

    private void HighlightToolbox()
    {
        HighlightControlNode(ToolBoxContainer);
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
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);

        GetViewport().SizeChanged += () => {
            Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);
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
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);

        GetViewport().SizeChanged += () => {
            Vector2 position = new Vector2(
                control.GlobalPosition.X - marginLeft + customXOffset - Camera.Position.X,
                control.GlobalPosition.Y - marginTop + customYOffset - Camera.Position.Y);
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

    #endregion

    private bool DoNotShowAgainWasChecked()
    {
        return File.Exists(doNotShowAgainMark);
    }

    private void SaveDoNotShowAgain()
    {
        if (!File.Exists(doNotShowAgainMark))
            File.Create(doNotShowAgainMark).Dispose();
    }

    private bool GetNextCondition()
    {
        var currentState = TutorialScenario[currentStateIndex];
        return currentState.CompletionCondition.Invoke();
    }

    private void OnYesButtonPress()
    {
        if (GetNextCondition())
        {
            GoToNextState();
        }
        else
        {
            //TODO: do something to indicate that condition isn't reached like highlight yes red or something
        }
            
        // TODO: goes to next slide or if end of the line quits
    }
    private void OnNoButtonPress()
    {
        QuitTutorial();
    }
    private void OnQuitButtonPress()
    {
        QuitTutorial();
    }
    private void OnNextButtonPress()
    {
        if (GetNextCondition())
        {
            GoToNextState();
        }
        else
        {
            //TODO: do something to indicate that condition isn't reached like highlight next red or something
        }
    }
    private void OnSkipButtonPress()
    {
        GoToNextState();
    }
    
    private void OnFinishButtonPressed()
    {
        QuitTutorial();
    }

}

 


