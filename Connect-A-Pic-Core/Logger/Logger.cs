using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Logger
{
    public struct Log
    {
        public int Level;
        public string Message;
        public DateTime timeStamp;
        public string ClassName;
    }
    public class Logger 
    {
        public List<Log> Logs { get; set; }
    }
}
