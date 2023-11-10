using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class TemplateTileView : TextureRect
	{

		public override void _DropData(Vector2 position, Variant data)
		{ // you cannot drop something on a template tile
		}

		public override Variant _GetDragData(Vector2 position)
		{
			if( GetChild(0) is ComponentView template)
			{
                var newComponent = ComponentViewFactory.Instance.CreateComponentView(template.TypeNumber);
                return newComponent;
            } else
			{
				var exceptionText = "child of _GetDragData() is not of type ComponentView even though it should be";
                CustomLogger.PrintErr(exceptionText);
				throw new DataMisalignedException(exceptionText);
			}
		}

		public override void _GuiInput(InputEvent inputEvent)
		{
			// override so that the tile cannot use middle click and rightclick
		}
	}

}