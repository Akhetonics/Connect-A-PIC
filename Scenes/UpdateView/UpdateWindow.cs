using Chickensoft.GodotTestDriver.Util;
using Godot;
using System;
using System.Drawing;
using System.Text;

public partial class UpdateWindow : Control
{
    private RichTextLabel title;
    private RichTextLabel body;
    private Button btnUpdate;
    private Button btnCancel;
    private Control updateSection;
    private Control updateCompleted;
    private RichTextLabel currentVersionNumber;
    private RichTextLabel newVersionNumber;
    private TextureProgressBar progressBar;

    private float processingAnimationSpeed = 0.3f;
    private double trackedTime = 0;
    private bool animating = false;
    private int dots = 0;


    public override void _Ready()
    {
        title = GetNode<RichTextLabel>("%Title");
        body = GetNode<RichTextLabel>("%Body");
        btnUpdate = GetNode<Button>("%Update");
        btnCancel = GetNode<Button>("%Cancel");
        updateSection = GetNode<Control>("%UpdateSection");
        currentVersionNumber = GetNode<RichTextLabel>("%CurrentVersion");
        newVersionNumber = GetNode<RichTextLabel>("%NewVersion");
        progressBar = GetNode<TextureProgressBar>("%ProgressBar");

        body.Text = "";

        progressBar.Value = 0;
        progressBar.Visible = false;

        btnUpdate.Pressed += StartUpdateProcess;
        btnCancel.Pressed += OnCancelPressed;

        // these are called from another thread and we need to update gui elements from main thread
        UpdateManager.DownloadStarted += (s,e) => this.CallDeferred("DownloadStarted");
        UpdateManager.ProgressUpdated += (s, a) => this.CallDeferred("ProgressUpdated");
        UpdateManager.InstallerReady += (s, a) => this.CallDeferred("DownloadCompleted");

        UpdateManager.UpdateAvailable += (s, e) => SetupValues();
        if (UpdateManager.IsUpdateAvailable()) SetupValues();

        Visible = false;
    }

    public override void _Process(double delta) {
        if (animating) {
            trackedTime += delta;
            if (trackedTime > processingAnimationSpeed) {
                trackedTime = 0;
                dots = (1 + dots) % 4;
                body.Text = "[center]Processing[/center]" + new string('.', dots);
            }
        }
    }

    private void OnUpdateButtonPressed() {
        Visible = !Visible;
    }

    private void OnCancelPressed() {
        Visible = false;
    }
    private void StartUpdateProcess() {
        if (UpdateManager.IsUpdateAvailable()) {
            UpdateManager.Instance.Update();

            animating = true;
            btnUpdate.Disabled = true;
            updateSection.Visible = false;
            body.Text = "[center]Processing...[/center]";
            title.Text = "[center]Connecting to internet[/center]";
        }
    }

    private void SetupValues() {
        updateSection.Visible = true;
        currentVersionNumber.Text = "[center][color=#FFC826]" + FormatVersionToString(UpdateManager.CurrentVersion) + "[/color][/center]";
        newVersionNumber.Text = "[center][color=#44FF44]" + FormatVersionToString(UpdateManager.LatestVersion) + "[/color][/center]";
    }
    private void DownloadStarted() {
        btnUpdate.Pressed -= StartUpdateProcess;
        title.Text = "[center]Please wait\nDownloading update[/center]";
        body.Text = "";
        animating = false;
        progressBar.Visible = true;
        updateSection.Visible = false;
        btnUpdate.Disabled = true;
    }
    private void ProgressUpdated() {
        progressBar.Value = UpdateManager.Progress * 100;
    }
    private void DownloadCompleted() {
        btnUpdate.Disabled = false;
        btnUpdate.Text = "Yes";
        btnCancel.Text = "No";
        title.Text = "[center][color=#44FF44]Update installed![/color]\n[color=#FFC826]Restart required.[/color][/center]";
        body.Text = "[center]Restart now?[/center]";
        progressBar.Visible = false;

        btnUpdate.Pressed -= StartUpdateProcess;
        btnUpdate.Pressed += CloseProgramAndStartInstaller;
    }

    private void CloseProgramAndStartInstaller() {
        UpdateManager.RunInstaller();
        System.Environment.Exit(0);
    }
    private static String FormatVersionToString(Version version) {
        StringBuilder sb = new StringBuilder();
        sb.Append(version.Major);
        sb.Append(".");
        sb.Append(version.Minor);
        sb.Append(".");
        sb.Append(version.Build);

        return sb.ToString();
    }
}
