using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper.DTOs;
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
        [Export] private Script ComponentBaseScriptPath;
		private List<PackedScene> PackedComponentScenes;
		private static ComponentViewFactory instance { get; set; }
		private Dictionary<int, (PackedScene scene, ComponentDraft componentDraft)> packedComponentCache = new();
		public static ComponentViewFactory Instance
		{
			get { return instance; }
		}

		public override void _Ready()
		{
			if (instance == null)
            {
                instance = this;
                LoadAllScenesFromSceneFolder();
                EmitSignal(nameof(InitializedEventHandler).Replace("EventHandler", ""));
            }
            else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}

        public void LoadAllScenesFromSceneFolder(string scenePath )
        {
            PackedComponentScenes = new List<PackedScene>();
            var folderPath = "Scenes\\Components";

            foreach (string sceneFile in Directory.EnumerateFiles(folderPath, "*.tscn", SearchOption.AllDirectories))
            {
                string godotPath = sceneFile.Replace(folderPath, "res://Scenes/Components").Replace("\\", "/");
                PackedScene scene = GD.Load<PackedScene>(godotPath);
                PackedComponentScenes.Add(scene);
            }
        }

        public void InitializeComponentDrafts(Dictionary<int, ComponentSceneAndDraft> packedComponentScenes)
        {
            // die Factories müssen bestückt werden mit einer gemeinsamen ID für die zu erstellenden Components und mit einer Schablone um zu wissen, was sie erstellen sollen.
            // im Falle der View wäre dsa eine PackedScene, einer int und auch der Data die in der PackedScene eingestellt werden muss. 
            
            foreach (var componentTemplate in packedComponentScenes)
            {
                var draftPackedScene = componentTemplate.Value.Scene;
                var draftData = componentTemplate.Value.ComponentDraft;
                var mainNode = draftPackedScene.Instantiate();
                mainNode.SetScript(ComponentBaseScriptPath);

                
                packedComponentCache.Add(mainNode.GetType(), componentTemplate);
                mainNode.QueueFree();
            }
        }

        public ComponentBaseView CreateComponentView(int componentNR )
		{
            var mainNode = packedComponentCache[componentNR];
            if (mainNode is ComponentBaseView view)
            {
                var slotDataSets = new List<AnimationSlotOverlayData>();
                foreach (Overlay overlay in draftData.overlays)
                {
                    slotDataSets.Add(new AnimationSlotOverlayData()
                    {
                        LightFlowOverlay = ResourceLoader.Load<Texture>(overlay.overlayAnimTexturePath),
                        OffsetX = overlay.tileOffsetX,
                        OffsetY = overlay.tileOffsetY,
                        Side = overlay.rectSide
                    });
                }
                view.InitializeComponent(slotDataSets, draftData.widthInTiles, draftData.heightInTiles);
            }
            else
            {
                CustomLogger.PrintErr($"ComponentTemplate is not of type ComponentBaseView: {draftData.identifier} {draftData.}");
                continue;
            }
            
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
    public record struct ComponentSceneAndDraft(PackedScene Scene, ComponentDraft ComponentDraft)
    {
        public static implicit operator (PackedScene scene, ComponentDraft componentDraft)(ComponentSceneAndDraft value)
        {
            return (value.Scene, value.ComponentDraft);
        }

        public static implicit operator ComponentSceneAndDraft((PackedScene scene, ComponentDraft componentDraft) value)
        {
            return new ComponentSceneAndDraft(value.scene, value.componentDraft);
        }
    }
}
