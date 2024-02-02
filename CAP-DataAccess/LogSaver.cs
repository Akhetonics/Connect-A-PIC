using CAP_Contracts.Logger;

namespace CAP_DataAccess
{
    public class LogSaver
    {
        public const int MaxLogFileSize = 1024 * 1024 * 20; // the last number is the megabytes 
        public const int MaxBackupFiles = 5;
        private readonly object _logFileLock = new object();
        public LogSaver(ILogger logger)
        {
            Logger = logger;
            Logger.LogAdded += SaveLog;
        }
        private void SaveLog(Log log)
        {
            lock (_logFileLock)
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolder = Path.Combine(appDataFolder, "Connect A PIC");
                string logFilePath = Path.Combine(appFolder, "logs.txt");
                string logBackupPathFormat = Path.Combine(appFolder, "logs_{0}.txt"); // Format for backup files for string.Format()

                Directory.CreateDirectory(appFolder);

                if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > MaxLogFileSize)
                {
                    RolloverLogs(logFilePath, logBackupPathFormat, MaxBackupFiles);
                }

                File.AppendAllText(logFilePath, log.TimeStamp + " " + log.Message + Environment.NewLine);
            }
        }

        private void RolloverLogs(string currentLogPath, string backupPathFormat, int maxBackups)
        {
            // Delete the oldest log file if the maximum number of backups is reached
            string oldestBackup = string.Format(backupPathFormat, maxBackups);
            if (File.Exists(oldestBackup))
            {
                File.Delete(oldestBackup);
            }

            // Shift each backup file up by one
            for (int i = maxBackups - 1; i >= 1; i--)
            {
                string sourcePath = string.Format(backupPathFormat, i);
                string destPath = string.Format(backupPathFormat, i + 1);
                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destPath);
                }
            }

            // Rename current log file to the first backup
            File.Move(currentLogPath, string.Format(backupPathFormat, 1));
        }
        public ILogger Logger { get; }
    }
}
