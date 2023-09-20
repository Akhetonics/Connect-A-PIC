using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class ComponentViewFactory : Node
	{
		private List<ComponentBaseView> AllComponentViewBlueprints;
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
			AllComponentViewBlueprints = GetAllComponentBaseViewNodes();
		}

		public List<ComponentBaseView> GetAllComponentBaseViewNodes()
		{
			List<ComponentBaseView> resultList = new List<ComponentBaseView>(); // this should load all scenes from the components folder
			foreach (Node child in GetChildren())
			{
				if (child is ComponentBaseView componentBase)
				{
					resultList.Add(componentBase);
				}
			}
			return resultList;
		}
		public ComponentBaseView CreateComponentView(Type ViewTypeListedInFactoryChildren)
		{
			if (! typeof(ComponentBaseView).IsAssignableFrom(ViewTypeListedInFactoryChildren)){
				throw new ArgumentException($"Type is not of ComponentBaseView: {nameof(ViewTypeListedInFactoryChildren) + " " + ViewTypeListedInFactoryChildren.FullName}");
			}
			foreach (ComponentBaseView component in AllComponentViewBlueprints)
			{
				if (ViewTypeListedInFactoryChildren == component.GetType())
				{
					var item = component.Duplicate();
					item._Ready();
					return item;
				}
			}
			throw new ComponentTemplateMissingException(ViewTypeListedInFactoryChildren.FullName);
		}
	}
}
