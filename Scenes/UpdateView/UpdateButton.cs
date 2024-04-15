using Godot;
using System;

public partial class UpdateButton : Button
{
    [Export] public Texture2D CompletedTexture;
    [Export] public Godot.Collections.Array<Texture2D> AnimationFrames;

    private bool updating = false;
    private double animationSpeed = 0.4f;
    private double timeTracked = 0;
    private int animationFrame = 0;

	public override void _Ready()
	{
        Visible = false;
        UpdateManager.UpdateAvailable += (s, e) => ShowSelf();
        if (UpdateManager.IsUpdateAvailable()) ShowSelf();

        UpdateManager.DownloadStarted += DownloadStarted;
        UpdateManager.DownloadCompleted += (s,e) => this.CallDeferred("UpdateReady");

    }

    private void UpdateReady() {
        updating = false;
        Icon = CompletedTexture;
    }

    private void DownloadStarted(object sender, EventArgs e) {
        updating = true;
    }

    public override void _Process(double delta) {
        if (!updating) return;

        timeTracked += delta;
        if (timeTracked > animationSpeed) {
            timeTracked = 0;
            Icon = AnimationFrames[animationFrame];
            animationFrame++;
            animationFrame %= AnimationFrames.Count;
        }
    }


    public void ShowSelf(){
        Visible = true;
    }
}
