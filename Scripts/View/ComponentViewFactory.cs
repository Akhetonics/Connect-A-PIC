using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class ComponentViewFactory : Node
	{
        [Signal] public delegate void InitializedEventHandler();
		private List<PackedScene> PackedComponentScenes;
		private static ComponentViewFactory instance { get; set; }
		private Dictionary<Type, PackedScene> packedComponentCache = new();
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
                foreach (string sceneFile in Directory.EnumerateFiles(folderPath, "*.tscn", SearchOption.AllDirectories))
                {
                    string godotPath = sceneFile.Replace(folderPath, "res://Scenes/Components").Replace("\\", "/");
                    PackedScene scene = GD.Load<PackedScene>(godotPath);
                    PackedComponentScenes.Add(scene);
                }
                CachePackedSceneTypes();
                EmitSignal(nameof(InitializedEventHandler).Replace("EventHandler",""));
            }
            else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}

        private void CachePackedSceneTypes()
        {
            packedComponentCache = new Dictionary<Type, PackedScene>();
            foreach (PackedScene componentTemplate in PackedComponentScenes)
            {
                var mainNode = componentTemplate.Instantiate();
                mainNode.SetScript(new ComponentBaseView());
                if (mainNode == null)
                {
                    CustomLogger.PrintErr($"ComponentTemplate is not of type ComponentBaseView: {componentTemplate.ResourcePath}");
                    continue;
                }
                packedComponentCache.Add(mainNode.GetType(), componentTemplate);
                mainNode.QueueFree();
            }
        }

        public ComponentBaseView CreateComponentView(Type ViewTypeListedInFactoryChildren)
		{
			if (! typeof(ComponentBaseView).IsAssignableFrom(ViewTypeListedInFactoryChildren)){
				CustomLogger.PrintErr($"Type is not of ComponentBaseView: {nameof(ViewTypeListedInFactoryChildren) + " " + ViewTypeListedInFactoryChildren.FullName}");
                throw new ArgumentException(nameof(ViewTypeListedInFactoryChildren));
			}
            try
            {
                var packedComponent = packedComponentCache.Single(c => c.Key == ViewTypeListedInFactoryChildren).Value;
                return packedComponent.Instantiate() as ComponentBaseView;
            } catch (Exception ex)
            {
                CustomLogger.PrintErr($"ComponentTemplate is not or not well defined: {ViewTypeListedInFactoryChildren.FullName} - Exception: {ex.Message}" );
                throw;
            }
        }

        public List<Type> GetAllComponentTypes()
        {
            return packedComponentCache.Keys.ToList();
        }
	}
}
