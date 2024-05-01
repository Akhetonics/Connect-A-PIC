using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using Octokit;
using GithubReleaseDownloader.Entities;
using GithubReleaseDownloader;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ConnectAPic.LayoutWindow;

public partial class UpdateManager : Node
{
    private static string InstallerPath =
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "installers");

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
        CurrentVersion = GameManager.Version;

        Directory.CreateDirectory(InstallerPath);

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
        var release = ReleaseManager.Instance.GetLatest(RepoOwnerName, RepoName);

        if (release is null) return;
        AssetDownloader.Instance.DownloadAllAssets(release, InstallerPath, progressChanged: RunOnProgressChanged);
    }

    private void RunOnProgressChanged(DownloadInfo downloadInfo) {
        if (downloadInfo.DownloadPercent == 1.0) {
            installerName = downloadInfo.Name;

            // set installer name to latest version
            string newFilePath = Path.Combine(InstallerPath, LatestVersion.ToString(), ".msi");
            File.Move(Path.Combine(InstallerPath, installerName), newFilePath);

            DownloadCompleted?.Invoke(this, EventArgs.Empty);
        }

        if (!startedUpdate){
            startedUpdate = true;
            DownloadStarted.Invoke(this, EventArgs.Empty);
        }

        Progress = (float) downloadInfo.DownloadPercent;
        ProgressUpdated?.Invoke(this, EventArgs.Empty);
    }

    private async Task CheckForUpdates()
    {
        var releases = await Client.Repository.Release.GetAll(RepoOwnerName, RepoName);
        var vers = releases[0].TagName.Replace("v", "");
        LatestVersion = new Version(vers);

        if (LatestVersion == null || CurrentVersion > LatestVersion) return; // Update not needed

        // if installer is downloaded then run installation
        if (File.Exists(Path.Combine(InstallerPath, LatestVersion.ToString(), ".msi")))
        {
            RunInstaller(Path.Combine(InstallerPath, LatestVersion.ToString(), ".msi"));
            System.Environment.Exit(0);
        }

        UpdateAvailable?.Invoke(null, EventArgs.Empty);

    }

    public static bool IsUpdateAvailable(){
        return
            CurrentVersion != null &
            LatestVersion != null &
            LatestVersion > CurrentVersion;
    }
    public static void RunInstaller(String installerPath = "")
    {
        if (string.IsNullOrEmpty(installerPath))
            installerPath = Path.Combine(InstallerPath, installerName);

        Process p = new Process();
        p.StartInfo.FileName = "msiexec";
        p.StartInfo.Arguments = "/i " + Path.Combine(installerPath);
        p.Start();
    }
}
