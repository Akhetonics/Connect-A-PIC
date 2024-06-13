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
    private static string RepoOwnerName = "Akhetonics";
    private static string RepoName = "Connect-A-PIC";

    private static string InstallerPath =
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), RepoOwnerName, RepoName, "installers");

    public static event EventHandler UpdateAvailable;
    public static event EventHandler DownloadStarted;
    public static event EventHandler ProgressUpdated;
    public static event EventHandler DownloadCompleted;
    public static event EventHandler InstallerReady;

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

        DownloadCompleted += (s, e) => RenameInstallerFile();

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
        if (File.Exists(Path.Combine(InstallerPath, $"{RepoName}_{LatestVersion}.msi")))
        {
            RunInstaller(Path.Combine(InstallerPath, $"{RepoName}_{LatestVersion}.msi"));
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

        ResetTutorial();

        Process p = new Process();
        p.StartInfo.FileName = "msiexec";
        p.StartInfo.Arguments = "/i " + Path.Combine(installerPath);
        p.Start();
    }

    private static void RenameInstallerFile() {
        var newInstallerName = $"{RepoName}_{LatestVersion}.msi";
        if (installerName == newInstallerName)
        {
            InstallerReady?.Invoke(null, EventArgs.Empty);
            return;
        }
        string newFilePath = Path.Combine(InstallerPath, newInstallerName);

        File.Move(Path.Combine(InstallerPath, installerName), newFilePath);
        installerName = newInstallerName;

        InstallerReady?.Invoke(null, EventArgs.Empty);
    }

    private static void ResetTutorial()
    {
        var tutorialResetFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), RepoOwnerName, RepoName, "doNotShowTutorial");

        if(File.Exists(tutorialResetFile))
            File.Delete(tutorialResetFile);
    }
}
