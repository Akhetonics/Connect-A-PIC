using CAP_Contracts.Logger;

namespace CAP_DataAccess
{
    public class LogSaver
    {
        public LogSaver(ILogger logger)
        {
            Logger = logger;
            Logger.LogAdded += SaveLog;
        }
        private void SaveLog(Log log)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataFolder, "Connect-A-PIC");
            string logFilePath = Path.Combine(appFolder, "logs.txt");

            // Ensure the directory exists
            Directory.CreateDirectory(appFolder);

            // Append the log text to the log file
            File.AppendAllText(logFilePath, log.Message + Environment.NewLine);
        }
        public ILogger Logger { get; }
    }
}
