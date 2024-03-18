using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public interface ITool
    {
        public void Update();
        public void Activate(ToolViewModel toolManager);
        public Guid GetID();
        public void Free();
    }
    public interface IToolPreviewable: ITool
    {
        public TemplateTileView CreateIcon();
    }
}
