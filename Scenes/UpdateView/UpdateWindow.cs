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

        UpdateManager.DownloadStarted += (s, a) => {
            updateSection.Visible = false;
            btnUpdate.Disabled = true;
            btnUpdate.Pressed -= StartUpdateProcess;
            progressBar.Visible = true;
        };

        UpdateManager.ProgressUpdated += (s, a) => {
            progressBar.Value = UpdateManager.Progress;
        };

        UpdateManager.DownloadCompleted += (s, a) => {
            btnUpdate.Disabled = false;
            btnUpdate.Text = "Yes";
            btnCancel.Text = "No";
            title.Text = "[center][color=#44FF44]Update installed![/color]\n[color=#FFC826]Restart required.[/color][/center]";
            body.Text = "[center]Restart now?[/center]";
            progressBar.Visible = false;

            btnUpdate.Pressed -= StartUpdateProcess;
            btnUpdate.Pressed += CloseProgramAndStartInstaller;
        };

        UpdateManager.UpdateAvailable += (s, e) => SetupValues();
        if (UpdateManager.IsUpdateAvailable()) SetupValues();
    }

    private void SetupValues() {
        currentVersionNumber.Text = "[center][color=#FFC826]" + FormatVersionToString(UpdateManager.CurrentVersion) + "[/color][/center]";
        newVersionNumber.Text = "[center][color=#44FF44]" + FormatVersionToString(UpdateManager.LatestVersion) + "[/color][/center]";
    }

    private void OnUpdateWindowPressed() {
        Visible = !Visible;
    }

    private void OnCancelPressed() {
        Visible = false;
    }

    private void StartUpdateProcess() {
        if (UpdateManager.IsUpdateAvailable())
            UpdateManager.Instance.Update();
    }

    private void CloseProgramAndStartInstaller(){
        UpdateManager.OpenInstaller();
        System.Environment.Exit(0);
    }

    private static String FormatVersionToString(Version version){
        StringBuilder sb = new StringBuilder();
        sb.Append(version.Major);
        sb.Append(".");
        sb.Append(version.Minor);
        sb.Append(".");
        sb.Append(version.Build);

        return sb.ToString();
    }
}



