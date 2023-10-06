using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class ComponentViewFactory : Node
	{
		[Export] public PackedScene[] PackedComponentScenes;
		private static ComponentViewFactory instance { get; set; }
		public static ComponentViewFactory Instance
		{
			get { return instance; }
		}

		public override void _Ready()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
				return;
			}
		}

		public ComponentBaseView CreateComponentView(Type ViewTypeListedInFactoryChildren)
		{
			if (! typeof(ComponentBaseView).IsAssignableFrom(ViewTypeListedInFactoryChildren)){
				CustomLogger.PrintErr($"Type is not of ComponentBaseView: {nameof(ViewTypeListedInFactoryChildren) + " " + ViewTypeListedInFactoryChildren.FullName}");
                throw new ArgumentException(nameof(ViewTypeListedInFactoryChildren));
			}
			foreach (PackedScene componentTemplate in PackedComponentScenes)
			{
                ComponentBaseView mainNode = componentTemplate.Instantiate() as ComponentBaseView;
				if(mainNode == null)
				{
					CustomLogger.PrintErr($"ComponentTemplate is not of type ComponentBaseView: {componentTemplate.ResourcePath}");
					continue;
				}
				if (ViewTypeListedInFactoryChildren == mainNode.GetType())
				{
					return mainNode;
				}
			}
			CustomLogger.PrintErr($"ComponentTemplate is not defined: {ViewTypeListedInFactoryChildren.FullName}");
            throw new ComponentTemplateMissingException(ViewTypeListedInFactoryChildren.FullName);
		}
	}
}
