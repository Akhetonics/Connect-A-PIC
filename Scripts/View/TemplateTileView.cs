using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class TemplateTileView : TextureRect
	{
		[Export] public ComponentViewFactory componentViewFactory { get; set; }
		public override void _Ready()
		{
			base._Ready();
			this.CheckForNull(x => x.componentViewFactory);
		}
		public override void _DropData(Vector2 position, Variant data)
		{ // you cannot drop something on a template tile
		}

		public override Variant _GetDragData(Vector2 position)
		{
			if( GetChild(0) is ComponentView template)
			{
				var newComponent = componentViewFactory.CreateComponentView(template.TypeNumber);
				return newComponent;
			} else
			{
				var exceptionText = "child of _GetDragData() is not of type ComponentView even though it should be";
				GameManager.Instance.Logger.PrintErr(exceptionText);
				throw new DataMisalignedException(exceptionText);
			}
		}

		public override void _GuiInput(InputEvent inputEvent)
		{
			// override so that the tile cannot use middle click and rightclick
		}
	}

}
