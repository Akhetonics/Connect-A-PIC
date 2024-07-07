using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


partial class InitialTutorial : ITutorialScenario
{
    [Export] MainCamera Camera { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Node2D PortContainer { get; set; }
    [Export] Control ToolBoxContainer { get; set; }
    [Export] TutorialPopup TutorialPopup { get; set; }
    [Export] HighlightingAreaController ExclusionControl { get; set; }


    /// <summary>
    /// Describes tutorial scenario, tutorial starts from 0th element to the end
    /// each tutorial state defines tutorial stage
    /// </summary>
    private List<TutorialState> TutorialStates { get; set; } = new List<TutorialState>();

    private int currentStateIndex = 0;

    private int portContainerOffset = 124;
    private int portsWidth = 120;
    private int portHeight = 62;

    /// <summary>
    /// Represents global position of top left corner of the grid
    /// </summary>
    private Vector2 gridGlobalPosition;

    /// <summary>
    /// Represents the view port position of menu buttons (default left top corner)
    /// </summary>
    private Vector2 menuButtonPosition = Vector2.Zero;

    public void Initialize(TutorialPopup tutorialPopup,
        HighlightingAreaController exclusionControl)
    {
        TutorialPopup = tutorialPopup;
        ExclusionControl = exclusionControl;
    }

    public override void SetupTutorial()
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
            Camera.RecenterCamera();

        };
        TutorialStates.Add(welcome);

        var explanation = new TutorialState(
            WindowPlacement.Center,
            ButtonsConfiguration.QuitNext,
            "What is Connect-A-PIC",
            "Connect-A-PIC (photonic integrated circuits) aims to simplify the design of optical circuits on a chip",
            () => true
            );
        TutorialStates.Add(explanation);

        var workingArea = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Main Circuit",
            "This is your main workspace, where you can design and build your own photonic circuits",
            () => true
            );
        workingArea.AddHighlightedElemenet(
            new HighlightedElement<Node2D>(PortContainer)
            .SetOffsets(2, 0).SetSize(1485, 743)); //TODO: move magic numbers out
        workingArea.FunctionWhenLoading = () =>
        {
            (ToolBoxContainer as ToolBoxCollapseControl)?.SetToolBoxToggleState(true);
        };
        workingArea.FunctionWhenUnloading = () =>
        {
            (ToolBoxContainer as ToolBoxCollapseControl)?.SetToolBoxToggleState(false);
        };
        TutorialStates.Add(workingArea);

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
        TutorialStates.Add(InputOutputs);

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
        var inpuPortsHighlight = new HighlightedElement<Node2D>(PortContainer)
            .SetOffsets(-portsWidth, portContainerOffset)
            .SetSize(portsWidth, portHeight * 3);
        InputPorts.HighlightedNodes.Add(inpuPortsHighlight);
        TutorialStates.Add(InputPorts);

        var OutputPorts = new TutorialState(
            WindowPlacement.TopRight,
            ButtonsConfiguration.QuitNext,
            "Output Ports",
            "The [color=FFD700]Output Ports[/color] give you the ability to read signal strength and phase shift, think of them as power meters",
            () => true
            );
        var outputPortsHighlight = new HighlightedElement<Node2D>(PortContainer)
            .SetOffsets(-portsWidth, portContainerOffset + portHeight * 3)
            .SetSize(portsWidth, portHeight * 5);
        OutputPorts.HighlightedNodes.Add(outputPortsHighlight);
        TutorialStates.Add(OutputPorts);

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
        TutorialStates.Add(ToolBox);

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
        TutorialStates.Add(Menu);

        #endregion

        var Finished = new TutorialState(
            WindowPlacement.Center,
            ButtonsConfiguration.Finish,
            "Tutorial Completed!",
            "Congratulations you've completed tutorial!\n" +
            "[color=FFD700]to open cheat sheet for controls press \"?\" on menu bar[/color]",
            () => true
            );
        TutorialStates.Add(Finished);
        Finished.FunctionWhenUnloading = () =>
        {
            Camera.autoCenterWhenResizing = false;
            Camera.noZoomingOrMoving = false;
        };

        GoToNext();
    }
    public override void GoToNext()
    {
        if (TutorialStates.Count == 0)
            throw new Exception("Tutorial scenario not defined!");

        if (currentStateIndex == TutorialStates.Count - 1)
        {
            QuitTutorial();
            return;
        }

        if (currentStateIndex > 0)
        {
            var currentState = TutorialStates[currentStateIndex];
            currentState.RunUnloadFunction();
        }

        currentStateIndex++;

        var newTutorialState = TutorialStates[currentStateIndex];

        SetupTutorialFrom(newTutorialState);
    }
    public override void QuitTutorial()
    {
        var currentState = TutorialStates[currentStateIndex];
        currentState.RunUnloadFunction();

        Camera.autoCenterWhenResizing = false;
        Camera.noZoomingOrMoving = false;

        currentStateIndex = -1;
    }
    public override bool GoToNextIfNextConditionSatisfied()
    {
        if (IsNextConditionSatisfied())
        {
            GoToNext();
            return true;
        }

        return false;
    }
    public override bool IsNextConditionSatisfied()
    {
        var currentState = TutorialStates[currentStateIndex];
        return currentState.CompletionCondition.Invoke();
    }
    public override void ResetTutorial()
    {
        currentStateIndex = -1;
        ExclusionControl.ClearExclusionZones();
    }

    private void SetupTutorialFrom(TutorialState state)
    {
        TutorialPopup.SetTitleText(state.Title);
        TutorialPopup.SetBodyText(state.Body);
        TutorialPopup.SetWindowPlacement(state.WindowPlacement);
        TutorialPopup.SetButtonConfiguration(state.ButtonsConfiguration);

        ExclusionControl.ClearExclusionZones();

        foreach (var highlightedControl in state.HighlightedControls)
        {
            ExclusionControl.SetCustomHighlight(highlightedControl,
                () => new Vector2(
                        highlightedControl.HighlightedNode.GlobalPosition.X - Camera.Position.X,
                        highlightedControl.HighlightedNode.GlobalPosition.Y - Camera.Position.Y)
                );
        }

        foreach (var highlightedNode in state.HighlightedNodes)
        {
            ExclusionControl.SetCustomHighlight(highlightedNode,
                () => new Vector2(
                        highlightedNode.HighlightedNode.GlobalPosition.X - Camera.Position.X,
                        highlightedNode.HighlightedNode.GlobalPosition.Y - Camera.Position.Y)
                );
        }

        state.RunSetupFunction();
    }

    private void HighlightGrid()
    {
        var element = new HighlightedElement<Node2D>(PortContainer).SetMargins(0).SetSize(1500, 743);
        ExclusionControl.SetCustomHighlight(element);
    }
    private void HighlightLeftPorts()
    {
        var element = new HighlightedElement<Node2D>(PortContainer)
            .SetOffsets(-portsWidth, portContainerOffset)
            .SetSize(portsWidth, portHeight * 8);
        ExclusionControl.SetCustomHighlight(element);
    }
    private void HighlightMenu()
    {
        ExclusionControl.SetCustomHighlight(
            new Vector2(MenuBar.Size.X + 6, MenuBar.Size.Y + 6),
            new Vector2(Camera.Position.X - 3, Camera.Position.Y - 3),
            () => new Vector2(Camera.Position.X - 3, Camera.Position.Y - 3)
            );
    }
    private void HighlightToolbox()
    {
        var element = new HighlightedElement<Control>(ToolBoxContainer);

        ExclusionControl.SetCustomHighlight(
            new Vector2(ToolBoxContainer.Size.X, ToolBoxContainer.Size.Y),
            new Vector2(GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X, GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y),
            () => new Vector2(GetViewport().GetVisibleRect().Size.X - ToolBoxContainer.Size.X, GetViewport().GetVisibleRect().Size.Y - ToolBoxContainer.Size.Y)
            );
    }

}

