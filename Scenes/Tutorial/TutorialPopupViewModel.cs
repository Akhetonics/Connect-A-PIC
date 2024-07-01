using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class TutorialPopupViewModel
{
    //TODO: need to move this somewhere where update manager can also access it
    private static string RepoOwnerName = "Akhetonics";
    private static string RepoName = "Connect-A-PIC";

    /// <summary>
    /// Used to determine if tutorial needs to be shown on startup again
    /// when file with this name is present in app data folder of user then tutorial shouldn't be shown again
    /// </summary>
    string doNotShowAgainMark = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), RepoOwnerName, RepoName, "doNotShowTutorial");

}
