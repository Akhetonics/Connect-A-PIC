using Godot;
using System;

public partial class UpdateButton : Button
{
	public override void _Ready()
	{
        Visible = false;
        UpdateManager.UpdateAvailable += (s, e) => ShowSelf();
        if (UpdateManager.IsUpdateAvailable()) ShowSelf();
    }

    public void ShowSelf(){
        Visible = true;
    }
}
