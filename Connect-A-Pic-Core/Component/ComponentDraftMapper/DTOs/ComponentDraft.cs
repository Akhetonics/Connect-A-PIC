using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Component.ComponentDraftMapper.DTOs
{
    public class ComponentDraft
    {
        public int fileFormatVersion { get; set; }
        public string identifier { get; set; }
        public string nazcaFunctionParameters { get; set; }
        public string nazcaFunctionName { get; set; }
        public int deltaLength { get; set; }
        public int widthInTiles { get; set; }
        public int heightInTiles { get; set; }
        public List<Pin> pins { get; set; }
        public List<Connection> connections { get; set; }
        public List<Overlay> overlays { get; set; }
    }
}
