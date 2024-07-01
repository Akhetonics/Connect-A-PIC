using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


partial class InitialTutorial : Node, ITutorialScenario
{
    [Export] MainCamera Camera { get; set; }
    [Export] Control MenuBar { get; set; }
    [Export] Node2D PortContainer { get; set; }
    [Export] Control ToolBoxContainer { get; set; }

    private TutorialPopupView TutorialPopup { get; set; }
    private ExclusionControl ExclusionControl { get; set; }

    /// <summary>
    /// Describes tutorial scenario, tutorial starts from 0th element to the end
    /// each tutorial state defines tutorial stage
    /// </summary>
    private List<TutorialState> TutorialStates { get; set; } = new List<TutorialState>();

    private int currentStateIndex = 0;
    

    public InitialTutorial(TutorialPopupView tutorialPopup, ExclusionControl exclusionControl)
    {
        TutorialPopup = tutorialPopup;
        ExclusionControl = exclusionControl;
    }

    public void SetupTutorial()
    {

    }

    public void GoToNext()
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
    public void QuitTutorial()
    {
        var currentState = TutorialStates[currentStateIndex];
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
    public void GoToNextIfNextConditionSatisfied()
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
    public bool IsNextConditionSatisfied()
    {
        var currentState = TutorialStates[currentStateIndex];
        return currentState.CompletionCondition.Invoke();
    }

    public void ResetTutorial()
    {
        throw new NotImplementedException();
    }

    private void SetupTutorialFrom(TutorialState state)
    {
        TutorialPopup.SetTitleText(state.Title);
        TutorialPopup.SetBodyText(state.Body);
        TutorialPopup.SetWindowPlacement(state.WindowPlacement);
        TutorialPopup.SetButtonConfiguration(state.ButtonsConfiguration);

        ExclusionControl.ClearExclusionZones();

        foreach (var HighlightedControl in state.HighlightedControls)
        {
            float customXSize = 0;
            float customYSize = 0;
            if (HighlightedControl.customSizeX == 0) customXSize = HighlightedControl.HighlightedNode.Size.X;
            if (HighlightedControl.customSizeY == 0) customYSize = HighlightedControl.HighlightedNode.Size.Y;

            if (HighlightedControl.customSizeX == 0 && HighlightedControl.customSizeY == 0)
            {

                ExclusionControl.SetCustomHighlight(


                    )

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

                exclusionZone.Size = new Vector2(customXSize + marginLeft + marginRight, customYSize + marginTop + marginBottom);
                exclusionZone.Visible = true;

                HighlightControlNode(HighlightedControl.HighlightedNode,
                    HighlightedControl.marginTop, HighlightedControl.marginRight, HighlightedControl.marginBottom, HighlightedControl.marginBottom,
                    HighlightedControl.OffsetX - cameraOffset.X, HighlightedControl.OffsetY - cameraOffset.Y);
            }
            else
            {
                HighlightControlNode(HighlightedControl.HighlightedNode,
                    HighlightedControl.marginTop, HighlightedControl.marginRight, HighlightedControl.marginBottom, HighlightedControl.marginBottom,
                    HighlightedControl.OffsetX - cameraOffset.X, HighlightedControl.OffsetY - cameraOffset.Y,
                    HighlightedControl.customSizeX, HighlightedControl.customSizeY);
            }
        }

        foreach (var HighlightedNode in state.HighlightedNodes)
        {
            HighlightControlNode(HighlightedNode.HighlightedNode,
                HighlightedNode.marginTop, HighlightedNode.marginRight, HighlightedNode.marginBottom, HighlightedNode.marginBottom,
                HighlightedNode.OffsetX - cameraOffset.X, HighlightedNode.OffsetY - cameraOffset.Y,
                HighlightedNode.customSizeX, HighlightedNode.customSizeY);
        }

        state.RunSetupFunction();
    }
}

