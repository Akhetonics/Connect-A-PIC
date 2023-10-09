using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class ComponentViewFactory : Node
	{
		private List<PackedScene> PackedComponentScenes;
		private static ComponentViewFactory instance { get; set; }
		private Dictionary<Type, ComponentBaseView> ComponentViewCache = new Dictionary<Type, ComponentBaseView>();
		public static ComponentViewFactory Instance
		{
			get { return instance; }
		}

		public override void _Ready()
		{
			if (instance == null)
            {
                instance = this;
                PackedComponentScenes = new List<PackedScene>();
                var folderPath = "Scenes\\Components";
                string[] sceneFiles = Directory.GetFiles(folderPath, "*.tscn", SearchOption.AllDirectories);
                foreach (string sceneFile in sceneFiles)
                {
                    string godotPath = sceneFile.Replace(folderPath, "res://Scenes/Components").Replace("\\", "/");
                    PackedScene scene = GD.Load<PackedScene>(godotPath);
                    PackedComponentScenes.Add(scene);
                }
                CachePackedSceneTypes();
            }
            else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
				return;
			}
		}

        private void CachePackedSceneTypes()
        {
            ComponentViewCache = new Dictionary<Type, ComponentBaseView>();
            foreach (PackedScene componentTemplate in PackedComponentScenes)
            {
                ComponentBaseView mainNode = componentTemplate.Instantiate() as ComponentBaseView;
                if (mainNode == null)
                {
                    CustomLogger.PrintErr($"ComponentTemplate is not of type ComponentBaseView: {componentTemplate.ResourcePath}");
                    continue;
                }
                ComponentViewCache.Add(mainNode.GetType(), mainNode);
                mainNode.QueueFree();
            }
        }

        public ComponentBaseView CreateComponentView(Type ViewTypeListedInFactoryChildren)
		{
			if (! typeof(ComponentBaseView).IsAssignableFrom(ViewTypeListedInFactoryChildren)){
				CustomLogger.PrintErr($"Type is not of ComponentBaseView: {nameof(ViewTypeListedInFactoryChildren) + " " + ViewTypeListedInFactoryChildren.FullName}");
                throw new ArgumentException(nameof(ViewTypeListedInFactoryChildren));
			}

			ComponentViewCache.Single(c => c.Key == ViewTypeListedInFactoryChildren);

            foreach (var componentCacheElement in ComponentViewCache)
			{
                componentCacheElement)
                ComponentBaseView mainNode = componentCacheElement.Instantiate() as ComponentBaseView;
				if(mainNode == null)
				{
					CustomLogger.PrintErr($"ComponentTemplate is not of type ComponentBaseView: {componentTemplate.ResourcePath}");
					continue;
				}
				if (ViewTypeListedInFactoryChildren == mainNode.GetType())
				{
					return mainNode;
				} else
				{
					mainNode.QueueFree();
				}
			}
			CustomLogger.PrintErr($"ComponentTemplate is not defined: {ViewTypeListedInFactoryChildren.FullName}");
            throw new ComponentTemplateMissingException(ViewTypeListedInFactoryChildren.FullName);
		}
	}
}
