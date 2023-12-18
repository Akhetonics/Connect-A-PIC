using System.Diagnostics;
using CAP_Contracts.Logger;

namespace CAP_Core
{
    public class Logger : ILogger
    {
        private List<Log> logs = new List<Log>();
        public event Action<Log> LogAdded;

        public Logger()
        {
            LogAdded = (Log) => Console.WriteLine(Log.ToString());
        }
        public Logger(Action<Log> logAddedAction)
        {
            this.LogAdded = logAddedAction;
        }
        public void Log(LogLevel level, string message)
        {
            var formattedMessage = FormatErrorText(message);
            var log = new Log
            {
                Level = level,
                Message = formattedMessage,
                TimeStamp = DateTime.Now,
                ClassName = GetCallingClassName(),
            };

            logs.Add(log);
            LogAdded?.Invoke(log);
        }

        private string FormatErrorText(string text)
        {
            try
            {
                StackTrace stackTrace = new(true);
                StackFrame frame = stackTrace.GetFrame(3);
                var method = frame.GetMethod();
                var lineNumber = frame.GetFileLineNumber();
                var declaringType = method.DeclaringType.Name;
                var methodName = method.Name;

                return $"{declaringType}.{methodName}:{lineNumber} > {text}";
            }
            catch (Exception ex)
            {
                return $"Error formatting log: {ex.Message}, Original log: {text}";
            }
        }

        private string GetCallingClassName()
        {
            StackTrace stackTrace = new(true);
            StackFrame frame = stackTrace.GetFrame(3);
            return frame.GetMethod().DeclaringType.Name;
        }

        public void PrintErr(string error)
        {
            Log(LogLevel.Error, error);
        }

        public void Print(string info)
        {
            Log(LogLevel.Info, info);
        }
    }
}
