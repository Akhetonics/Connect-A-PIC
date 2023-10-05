using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class TemplateTileView : TextureRect
	{
		[Export] private NodePath componentTemplatePath;
		private ComponentBaseView componentTemplate;
		public override void _Ready()
		{
			base._Ready();
			if (string.IsNullOrEmpty(componentTemplatePath)) CustomLogger.PrintErr(nameof(componentTemplatePath));
			var node = GetNode(componentTemplatePath); // not sure what is wrong here but it cannot convert the node to straightline.
			componentTemplate = (ComponentBaseView)node;
			if (componentTemplate == null) CustomLogger.PrintErr(new ArgumentNullException(nameof(componentTemplate)).ToString());
		}

		public override void _DropData(Vector2 position, Variant data)
		{ // you cannot drop something on a template tile
		}

		public override Variant _GetDragData(Vector2 position)
		{
			return componentTemplate;
		}

		public override void _GuiInput(InputEvent inputEvent)
		{
			// override so that the tile cannot use middle click and rightclick
		}
	}

}
