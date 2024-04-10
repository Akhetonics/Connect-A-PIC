using Godot;
using System;
using System.Drawing;
using System.Text;

public partial class UpdateWindow : Control
{
    RichTextLabel currentVersionNumber;
    RichTextLabel newVersionNumber;

	public override void _Ready()
	{
        currentVersionNumber = GetNode<RichTextLabel>("%CurrentVersion");
        newVersionNumber = GetNode<RichTextLabel>("%NewVersion");

        Visible = false;
        UpdateManager.UpdateAvalilable += (s, e) => SetupValues();
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

    private void OnUpdatePressed() {
        if (UpdateManager.IsUpdateAvailable())
            UpdateManager.Update();
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



