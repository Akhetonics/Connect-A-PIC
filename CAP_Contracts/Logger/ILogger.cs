namespace CAP_Contracts.Logger
{
    public interface ILogger
    {
        public void PrintErr(string error);
        public void PrintInfo(string info);
        public event Action<Log> LogAdded;
    }
}