using CAP_Contracts.Logger;
using CAP_Core.Helpers;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
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
        public GridView GridView { get; }
        public GridViewModel GridViewModel { get; private set; }
        public bool MiddleMouseButtonPressed { get; private set; }
        public bool WasMiddleMousePressedBefore { get; private set; }
        public Guid DeleteStrokeID { get; private set; }

        protected ToolBase(GridView gridView)
        {
            GridView = gridView;
            GridViewModel = GridView.ViewModel;
            GridView.AddChild(this);
        }
        protected virtual void Activate() { }
        protected virtual void FreeTool() { }
        public void ActivateTool()
        {
            IsActive = true;
            Activate();
        }

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

        protected Vector2I GetMouseGridPosition() => GetGridPosition(GridView.GetLocalMousePosition());
        protected static Vector2I GetGridPosition(Vector2 position)
        {
            var tileSize = (GameManager.TilePixelSize);
            return new Vector2I (((int)((position.X) / tileSize)), ((int)((position.Y) / tileSize)));
        }
        protected void HandleMiddleMouseDeleteDrawing(InputEvent @event)
        {
            if (IsActive == false) return;
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                if (mouseButtonEvent.ButtonIndex == MouseButton.Middle)
                {
                    MiddleMouseButtonPressed = mouseButtonEvent.Pressed;
                    DeleteComponentIfMiddleMouseIsPressed();
                }
            }
            // enabling mouse drag drawing
            else if (@event is InputEventMouseMotion)
            {
                DeleteComponentIfMiddleMouseIsPressed();
            }
        }

        private void DeleteComponentIfMiddleMouseIsPressed()
        {
            Vector2I gridPosition = GetMouseGridPosition();
            if (Input.IsMouseButtonPressed(MouseButton.Middle))
            {
                if( WasMiddleMousePressedBefore == false)
                {
                    WasMiddleMousePressedBefore = true;
                    DeleteStrokeID = Guid.NewGuid();
                }
                var delParams = new DeleteComponentArgs(new() { new IntVector(gridPosition.X, gridPosition.Y) }, DeleteStrokeID);
                GridViewModel.CommandFactory.CreateCommand(CommandType.DeleteComponent).ExecuteAsync(delParams).Wait();
            } else
            {
                WasMiddleMousePressedBefore = false;
            }
        }
    }
}
