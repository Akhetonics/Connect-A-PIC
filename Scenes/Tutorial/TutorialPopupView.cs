using Godot;
using System;

public partial class TutorialPopupView : Control
{
    [Export] RichTextLabel Title;
    [Export] RichTextLabel Body;
    [Export] Button NoShowAgainBtn;
    [Export] Control YesNoConfiguration;
    [Export] Control QuitSkipNextConfig;
    [Export] Control FinishConfig;
    [Export] Control SkipContainer;
    [Export] Control NextContainer;


    //Godot signal for easier handling from gui
    [Signal] public delegate void FinishPressedEventHandler();
    [Signal] public delegate void YesPressedEventHandler();
    [Signal] public delegate void NoPressedEventHandler();
    [Signal] public delegate void QuitPressedEventHandler();
    [Signal] public delegate void SkipPressedEventHandler();
    [Signal] public delegate void NextPressedEventHandler();

    public bool DoNotShowAgain
    {
        get => NoShowAgainBtn.ButtonPressed;
        set => NoShowAgainBtn.ButtonPressed = value;
    }


    private TutorialPopupViewModel viewModel = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AssignExportedPropertiesThatAreNull();

    }


    public void SetTitleText(string text)
    {
        Title.Text = $"[center]{text}[/center]";
    }
    public void SetBodyText(string text)
    {
        Body.Text = $"[center]{text}[/center]";
    }
    public void SetButtonConfiguration(ButtonsConfiguration configuration)
    {
        switch (configuration)
        {
            case ButtonsConfiguration.Finish: SetFinishConfiguration(); break;
            case ButtonsConfiguration.QuitNext: SetQuitNextConfiguration(); break;
            case ButtonsConfiguration.QuitSkip: SetQuitSkipConfiguration(); break;
            case ButtonsConfiguration.YesNo: SetYesNoConfiguration(); break;
        }
    }
    public void SetWindowPlacement(WindowPlacement placement)
    {
        switch (placement)
        {
            case WindowPlacement.Center: SetPositionToCenter(); break;
            case WindowPlacement.TopRight: SetPositionToTopRight(); break;
        }
    }

    private void SetPositionToCenter()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.Center, LayoutPresetMode.KeepSize);
    }
    private void SetPositionToTopRight()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.TopRight, LayoutPresetMode.KeepSize);
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

    private void AssignExportedPropertiesThatAreNull()
    {
        if (Title == null) Title = GetNode<RichTextLabel>("%Title");
        if (Body == null) Body = GetNode<RichTextLabel>("%Body");
        if (NoShowAgainBtn == null) NoShowAgainBtn = GetNode<Button>("%NoShowAgain");
        if (YesNoConfiguration == null) NoShowAgainBtn = GetNode<Button>("%YesNoConfiguration");
        if (QuitSkipNextConfig == null) NoShowAgainBtn = GetNode<Button>("%QuitSkipNextConfiguration");
        if (FinishConfig == null) NoShowAgainBtn = GetNode<Button>("%FinishConfiguration");
        if (SkipContainer == null) NoShowAgainBtn = GetNode<Button>("%NextContainer");
        if (NextContainer == null) NoShowAgainBtn = GetNode<Button>("%SkipContainer");
    }

    private void OnNoButtonPressed()     => EmitSignal(SignalName.NoPressed);
    private void OnYesButtonPressed()    => EmitSignal(SignalName.YesPressed); 
    private void OnQuitButtonPressed()   => EmitSignal(SignalName.QuitPressed);
    private void OnSkipButtonPressed()   => EmitSignal(SignalName.SkipPressed);
    private void OnNextButtonPressed()   => EmitSignal(SignalName.NextPressed);
    private void OnFinishButtonPressed() => EmitSignal(SignalName.FinishPressed);
}
