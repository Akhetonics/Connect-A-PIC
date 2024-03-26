using CAP_Contracts.Logger;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using SuperNodes.Types;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
    [SuperNode(typeof(Dependent))]
    public partial class TemplateTileView : TextureRect
    {
        //[Export] public ComponentViewFactory componentViewFactory { get; set; }
        public override partial void _Notification(int what);
        public EventHandler Clicked;
        [Dependency] public ComponentViewFactory ComponentViewFactory => DependOn<ComponentViewFactory>();
        [Dependency] public ILogger Logger => DependOn<ILogger>();
        public override void _Ready()
        {
            base._Ready();
        }
        public override void _DropData(Vector2 position, Variant data)
        { // you cannot drop something on a template tile
        }

        public override void _GuiInput(InputEvent inputEvent)
        {
            if( inputEvent is InputEventMouseButton mouseButtonEvent)
            {
                if(mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == MouseButton.Left)
                {
                    Clicked?.Invoke(this , new EventArgs());
                }
            }
        }
    }

}
