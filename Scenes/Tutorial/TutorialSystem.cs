using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scenes.InteractionOverlay;
using ConnectAPIC.Scenes.RightClickMenu;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class TutorialSystem : Control
{
    [Export] TutorialPopupView TutorialPopup { get; set; }
    [Export] ExclusionControl ExclusionControl { get; set; }

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
    /// when file with this name is present in app data folder of user then tutorial shouldn't be shown again
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

        TutorialPopup.SkipPressed += GoToNextState;

        TutorialPopup.YesPressed += GoToNextIfNextConditionSatisfied;
        TutorialPopup.NextPressed += GoToNextIfNextConditionSatisfied;

        TutorialPopup.NoPressed += QuitTutorial;
        TutorialPopup.QuitPressed += QuitTutorial;
        TutorialPopup.FinishPressed += QuitTutorial;


        string doNotRun = System.Environment.GetEnvironmentVariable("DO_NOT_RUN_TUTORIAL");
        if (doNotRun != null) return;

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
            ButtonsConfiguration.YesNo,
            "Welcome to Connect-A-PIC",
            "Want to go through a tutorial?",
            () => true
            );

        welcome.FunctionWhenLoading = () =>
        {
            TutorialPopup.DoNotShowAgain = true;
            Camera.autoCenterWhenResizing = true;
            Camera.noZoomingOrMoving = true;
        };

        TutorialScenario.Add(welcome);


        var explanation = new TutorialState(
            WindowPlacement.Center,
            ButtonsConfiguration.QuitNext,
            "What is Connect-A-PIC",
            "Connect-A-PIC (photonic integrated circuits) aims to simplify the design of optical circuits on a chip",
            () => true
            );

        TutorialScenario.Add(explanation);


        var workingArea = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Main Circuit",
            "This is your main workspace, where you can design and build your own photonic circuits",
            () => true
            );

        workingArea.HighlightedNodes.Add(new HighlightedElement<Node2D>
        {
            HighlightedNode = PortContainer,
            OffsetX = 2,
            customSizeX = 1485,
            customSizeY = 743
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
            ButtonsConfiguration.QuitNext,
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

        //ChangingPorts.HighlightedNodes.Add(new Highlighted<Node2D>
        //{
        //    HighlightedNode  = PortContainer,
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
        //            HighlightControlNode(ControlMenu.GetChild(0) as Control, marginTop: 10, marginRight: 20, marginBottom: 20, marginLeft: 10);
        //        }
        //    };
        //};

        //TutorialScenario.Add(ChangingPorts);
        #endregion

        var InputPorts = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Input Ports",
            "The [color=FFD700]Input Ports[/color] provide you with the photonic signal, think of them as power sources",
            () => true
            );

        InputPorts.HighlightedNodes.Add(new HighlightedElement<Node2D>
        {
            HighlightedNode = PortContainer,
            OffsetX = -portsWidth,
            OffsetY = portContainerOffset,
            customSizeX = portsWidth,
            customSizeY = portHeight * 3
        });

        TutorialScenario.Add(InputPorts);


        var OutputPorts = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Output Ports",
            "The [color=FFD700]Output Ports[/color] give you the ability to read signal strength and phase shift, think of them as power meters",
            () => true
            );

        OutputPorts.HighlightedNodes.Add(new HighlightedElement<Node2D>
        {
            HighlightedNode = PortContainer,
            OffsetX = -portsWidth,
            OffsetY = portContainerOffset + portHeight * 3,
            customSizeX = portsWidth,
            customSizeY = portHeight * 5
        });

        TutorialScenario.Add(OutputPorts);

        #endregion

        #region tool box explanation

        var ToolBox = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Tool Box",
            "Toolbox offers a wide range of components, you can left-click to select component, and left-click (or left-click and drag) to place it on the working grid",
            () => true
            );

        ToolBox.FunctionWhenLoading = () =>
        {
            ExclusionControl.SetCustomHighlight(
                    new Vector2(ToolBoxContainer.Size.X, ToolBoxContainer.Size.Y),
                    new Vector2(
                        GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X,
                        GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y),
                    () => {
                        return new Vector2(
                            GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X,
                            GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y);
                    }
                );
        };

        TutorialScenario.Add(ToolBox);

        #endregion

        #region menu bar explanation

        var Menu = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Menu Bar",
            "From the menu bar you can export/import your circuit, turn on/off your circuit, export your circuit to NAZCA and undo/redo your actions, also updates will appear here if available",
            () => true
            );

        Menu.FunctionWhenLoading = () =>
        {
            ExclusionControl.SetCustomHighlight(
                    new Vector2(MenuBar.Size.X + 6, MenuBar.Size.Y + 6),
                    new Vector2(Camera.Offset.X - 3, Camera.Offset.Y - 3),
                    () => new Vector2(Camera.Offset.X - 3, Camera.Offset.Y - 3)
                );
        };

        TutorialScenario.Add(Menu);

        #endregion

        var Finished = new TutorialState(
            WindowPlacement.Center,
            ButtonsConfiguration.Finish,
            "Tutorial Completed!",
            "Congratulations you've completed tutorial!\n" +
            "[color=FFD700]to open cheat sheet for controls press \"?\" on menu bar[/color]",
            () => true
            );

        TutorialScenario.Add(Finished);

        //last function should releases clicking and scrolling
        Finished.FunctionWhenUnloading = () =>
        {
            Camera.autoCenterWhenResizing = false;
            Camera.noZoomingOrMoving = false;
        };

        GoToNextState();
    }

    private void SetupTutorialFrom(TutorialState state)
    {
        TutorialPopup.SetTitleText(state.Title);
        TutorialPopup.SetBodyText(state.Body);
        TutorialPopup.SetWindowPlacement(state.WindowPlacement);
        TutorialPopup.SetButtonConfiguration(state.ButtonsConfiguration);

        ExclusionControl.ClearExclusionZones();

        ExclusionControl.HighlightFromTutorialState(state, Camera.Position);

        state.RunSetupFunction();
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

        if (TutorialPopup.DoNotShowAgain)
        {
            SaveDoNotShowAgain();
        }

        this.Visible = false;
    }
    private void GoToNextIfNextConditionSatisfied()
    {
        if (IsNextConditionSatisfied())
        {
            GoToNextState();
        }
        else
        {
            //TODO: do something to indicate that condition isn't reached like highlight yes red or something
        }
    }
    private bool IsNextConditionSatisfied()
    {
        var currentState = TutorialScenario[currentStateIndex];
        return currentState.CompletionCondition.Invoke();
    }

    private bool DoNotShowAgainWasChecked()
    {
        return File.Exists(doNotShowAgainMark);
    }
    private void SaveDoNotShowAgain()
    {
        if (!File.Exists(doNotShowAgainMark))
            File.Create(doNotShowAgainMark).Dispose();
    }

}

 


