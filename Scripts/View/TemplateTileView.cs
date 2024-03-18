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
            this.MouseFilter = MouseFilterEnum.Stop;
        }
        public override void _DropData(Vector2 position, Variant data)
        { // you cannot drop something on a template tile
        }

        //public override Variant _GetDragData(Vector2 position)
        //{
        //    if (GetChild(0) is ComponentView template)
        //    {
        //        var newComponent = ComponentViewFactory.CreateComponentView(template.ViewModel.TypeNumber);
        //        return newComponent;
        //    }
        //    else
        //    {
        //        var exceptionText = "child of _GetDragData() is not of type ComponentView even though it should be";
        //        Logger.PrintErr(exceptionText);
        //        throw new DataMisalignedException(exceptionText);
        //    }
        //}

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
