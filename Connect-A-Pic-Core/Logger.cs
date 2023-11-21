using Components.ComponentDraftMapper;
using CAP_Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAP_Contracts.Logger;

namespace CAP_Core
{
    public class Logger : ILogger
    {
        private List<Log> logs = new List<Log>();
        public event Action<Log> LogAdded;
        
        public void AddLog(LogLevel level, string message)
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
                StackFrame frame = stackTrace.GetFrame(3); // Anpassung je nach Aufrufkontext
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
            StackFrame frame = stackTrace.GetFrame(3); // Anpassung je nach Aufrufkontext
            return frame.GetMethod().DeclaringType.Name;
        }

        public void PrintErr(string error)
        {
            AddLog(LogLevel.Error, error);
        }

        public void PrintInfo(string info)
        {
            AddLog(LogLevel.Info, info);
        }
    }
}
