using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using Octokit;
using GithubReleaseDownloader.Entities;
using GithubReleaseDownloader;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class UpdateManager : Node
{
    private static string RepoOwnerName = "Akhetonics";
    private static string RepoName = "Connect-A-PIC";

    public static event EventHandler UpdateAvailable;
    public static event EventHandler DownloadStarted;
    public static event EventHandler ProgressUpdated;
    public static event EventHandler DownloadCompleted;

    public static UpdateManager Instance {  get; private set; }

    public static GitHubClient Client;
    public static Version CurrentVersion;
    public static Version LatestVersion;

    public static float Progress { get; private set; }
    private static string installerName;
    private static bool startedUpdate = false;

    public override void _Ready()
	{
        Client = new GitHubClient(new ProductHeaderValue(RepoName));
        CurrentVersion = GetType().Assembly.GetName().Version;

        Instance = this;

        _ = CheckForUpdates();
    }

    /// <summary>
    /// Initiates software update
    /// </summary>
    public void Update(){
        Task.Run(async () => await DownloadProcess());
    }

    public async Task DownloadProcess(){
        var downloadPath = Path.GetTempPath();

        var release = ReleaseManager.Instance.GetLatest(RepoOwnerName, RepoName);

        if (release is null) return;
        AssetDownloader.Instance.DownloadAllAssets(release, downloadPath, progressChanged: RunOnProgressChanged);
    }

    private void RunOnProgressChanged(DownloadInfo downloadInfo) {
        if (downloadInfo.DownloadPercent == 1.0) {
            installerName = downloadInfo.Name;
            DownloadCompleted?.Invoke(this, EventArgs.Empty);
        }

        if (!startedUpdate){
            startedUpdate = true;
            DownloadStarted.Invoke(this, EventArgs.Empty);
        }

        Progress = (float) downloadInfo.DownloadPercent;
        ProgressUpdated?.Invoke(this, EventArgs.Empty);
    }


    public static void OpenInstaller(){
        Process p = new Process();
        p.StartInfo.FileName = "msiexec";
        p.StartInfo.Arguments = "/i " + Path.Combine(Path.GetTempPath(), installerName);
        p.Start();
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
            UpdateAvailable?.Invoke(null, EventArgs.Empty);
        }
    }
}
