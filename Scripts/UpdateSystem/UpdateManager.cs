using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using Octokit;

public partial class UpdateManager : Node
{
    private static string RepoOwnerName = "Akhetonics";
    private static string RepoName = "Connect-A-PIC";

    public static event EventHandler UpdateAvalilable;

    public static GitHubClient Client;
    public static Version CurrentVersion;
    public static Version LatestVersion;

    public override void _Ready()
	{
        Client = new GitHubClient(new ProductHeaderValue(RepoName));
        CurrentVersion = GetType().Assembly.GetName().Version;

        _ = CheckForUpdates();
    }

    /// <summary>
    /// Initiates software update
    /// </summary>
    public static void Update(){
        //needs implementation
        GD.Print("Update called");
    }

    public static bool IsUpdateAvailable(){
        return
            CurrentVersion != null &
            LatestVersion != null &
            LatestVersion > CurrentVersion;
    }

    private async Task CheckForUpdates(){
        var releases = await Client.Repository.Release.GetAll(RepoOwnerName, RepoName);
        var vers = releases[0].TagName.Replace("v", "") + ".0";
        LatestVersion = new Version(vers);

        if (LatestVersion != null && CurrentVersion < LatestVersion){
            UpdateAvalilable.Invoke(this, EventArgs.Empty);
        }
    }
}
