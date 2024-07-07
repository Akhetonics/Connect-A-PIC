using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scenes.InteractionOverlay;
using ConnectAPIC.Scenes.RightClickMenu;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

public partial class TutorialSystem : Node
{
    [Export] TutorialPopup TutorialPopup { get; set; }
    [Export] HighlightingAreaController HighlightControl { get; set; }
    [Export] ITutorialScenario CurrentTutorial { get; set; }

    /// <summary>
    /// Used to determine if tutorial needs to be shown on startup again
    /// if file with this name is present in app data folder of user then tutorial shouldn't be shown again
    /// </summary>
    string doNotShowAgainMark =
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), GameManager.RepoOwnerName, GameManager.RepoName, "doNotShowTutorial");

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            CurrentTutorial.GoToNextIfNextConditionSatisfied();
        }
        else if (Input.IsActionJustPressed("ui_cancel"))
        {
            HandleQuittingWithDoNotShowAgain(TutorialPopup.DoNotShowAgain);
        }
    }

    public override void _Ready()
    {
        TutorialPopup.Hide();
        HighlightControl.Hide();

        // no tutorial
        if (CurrentTutorial == null) return;

        TutorialPopup.SkipPressed += CurrentTutorial.GoToNext;

        TutorialPopup.YesPressed += () => CurrentTutorial.GoToNextIfNextConditionSatisfied();
        TutorialPopup.NextPressed += () => CurrentTutorial.GoToNextIfNextConditionSatisfied();

        TutorialPopup.NoPressed += HandleQuittingWithDoNotShowAgain;
        TutorialPopup.QuitPressed += HandleQuittingWithDoNotShowAgain;
        TutorialPopup.FinishPressed += HandleQuittingWithDoNotShowAgain;

        // checks if tutorial should be run during testing (if DO_NOT_RUN_TUTORIAL evn variable exists we don't run tutorial)
        string doNotRun = System.Environment.GetEnvironmentVariable("DO_NOT_RUN_TUTORIAL");
        if (doNotRun != null) return;

        // if do not show mark is in user files we don't run tutorial
        if (DoNotShowMarkExists()) return;

        StartTutorial();
    }

    public void StartTutorial()
    {
        CurrentTutorial.ResetTutorial();
        CurrentTutorial.SetupTutorial();
        HighlightControl.Show();
        TutorialPopup.Show();
    }

    public void HandleQuittingWithDoNotShowAgain(bool doNotShowAgain)
    {
        if (doNotShowAgain && !DoNotShowMarkExists())
            SaveDoNotShowAgain();

        CurrentTutorial.QuitTutorial();
        HighlightControl.Hide();
    }

    private bool DoNotShowMarkExists()
    {
        return File.Exists(doNotShowAgainMark);
    }
    private void SaveDoNotShowAgain()
    {
        if (!File.Exists(doNotShowAgainMark))
            File.Create(doNotShowAgainMark).Dispose();
    }

}

 


