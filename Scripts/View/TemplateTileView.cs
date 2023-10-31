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
			var template = GetChild(0);
			var componentType = (template as ComponentBaseView)?.GetType();
			var newComponent = ComponentViewFactory.Instance.CreateComponentView(componentType);
			return newComponent;
		}

		public override void _GuiInput(InputEvent inputEvent)
		{
			// override so that the tile cannot use middle click and rightclick
		}
	}

}
