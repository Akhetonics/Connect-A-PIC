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
    public abstract partial class ToolBase : Node2D, ITool
    {
        public override partial void _Notification(int what);
        public const string ToolIDMetaName = "ToolID";
        [Dependency] public ILogger Logger => DependOn<ILogger>();
        public Guid ID { get; protected set; } = Guid.NewGuid();
        public bool IsActive { get; set; }

        protected virtual void Activate() { }
        protected virtual void FreeTool() { }
        // Template-Methode für Activate
        public void ActivateTool()
        {
            IsActive = true;
            Activate();
        }

        // Template-Methode für FreeTool
        public void DeactivateTool()
        {
            IsActive = false;
            FreeTool();
        }

        public static Guid GetToolIDFromPreview(Node preview)
        {
            var metaGuid = (string)preview.GetMeta(ToolIDMetaName) ?? "";
            return Guid.Parse(metaGuid);
        }

        public void OnResolved()
        {
            Logger.Print("Hallo");
        }
    }
}
