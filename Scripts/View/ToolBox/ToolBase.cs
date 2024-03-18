using CAP_Contracts.Logger;
using Chickensoft.AutoInject;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using SuperNodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    [SuperNode(typeof(Dependent))]
    public partial class ToolBase : Node2D, ITool
    {
        public override partial void _Notification(int what);
        protected const string ToolIDMetaName = "ToolID";
        [Dependency] public ILogger Logger => DependOn<ILogger>();
        protected Guid ID { get; set; }

        public ToolBase()
        {
            ID = Guid.NewGuid();
        }
        public void Activate(ToolViewModel toolManager)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public static Guid GetToolIDFromPreview(Node preview)
        {
            var metaGuid = (string)preview.GetMeta(ToolIDMetaName);
            return Guid.Parse(metaGuid);
        }
        public Guid GetID()
        {
            return ID;
        }
    }
}
