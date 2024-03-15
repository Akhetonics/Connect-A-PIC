using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Tools
{
    public class ToolManager
    {
        private List<ITool> Tools { get; set; } = new List<ITool>();
        private ITool CurrentTool { get; set; }

    }
}
