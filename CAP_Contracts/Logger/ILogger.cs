namespace CAP_Contracts.Logger
{
    public interface ILogger
    {
        public void PrintErr(string error);
        public void Print(string info);
        public void Log(LogLevel level, string message);
        public event Action<Log> LogAdded;
    }
}