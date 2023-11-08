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
        private Dictionary<int, ComponentSceneAndDraft> packedComponentCache = new();
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

        public List<PackedScene> LoadAllScenesFromSceneFolder()
        {
            PackedComponentScenes = new List<PackedScene>();
            var folderPath = "Scenes\\Components";

            foreach (string sceneFile in Directory.EnumerateFiles(folderPath, "*.tscn", SearchOption.AllDirectories))
            {
                string godotPath = sceneFile.Replace(folderPath, "res://Scenes/Components").Replace("\\", "/");
                PackedScene scene = GD.Load<PackedScene>(godotPath);
                PackedComponentScenes.Add(scene);
            }
            return PackedComponentScenes;
        }

        public void InitializeComponentDrafts(List<ComponentDraft> drafts)
        {
            Dictionary<int, ComponentSceneAndDraft> packedComponentScenes = new();

            int componentNumber = 0;
            foreach (var componentDraft in drafts)
            {
                PackedScene packedScene;
                try {
                    packedScene = GD.Load<PackedScene>(componentDraft.sceneResPath);
                } catch( Exception ex)
                {
                    CustomLogger.PrintErr($"Error Loading PackedScene '{componentDraft.sceneResPath}' of Comopnent: {componentDraft.identifier} ex: {ex.Message} )");
                    continue;
                }
                packedComponentScenes.Add(componentNumber, new ComponentSceneAndDraft()
                {
                    Draft = componentDraft,
                    Scene = packedScene
                });
                componentNumber++;
            }
            packedComponentCache = packedComponentScenes;
        }

        public ComponentBaseView CreateComponentView(int componentNR)
        {
            if (!packedComponentCache.ContainsKey(componentNR))
            {
                CustomLogger.PrintErr("Key does not exist in ComponentCache of ComponentviewFactory: " + componentNR);
            }
            var draft = packedComponentCache[componentNR].Draft;
            var packedScene = packedComponentCache[componentNR].Scene;
            var slotDataSets = new List<AnimationSlotOverlayData>();

            try
            {
                foreach (Overlay overlay in draft.overlays)
                {
                    slotDataSets.Add(new AnimationSlotOverlayData()
                    {
                        LightFlowOverlay = ResourceLoader.Load<Texture>(overlay.overlayAnimTexturePath),
                        OffsetX = overlay.tileOffsetX,
                        OffsetY = overlay.tileOffsetY,
                        Side = overlay.rectSide
                    });
                }
                var view = packedScene.Instantiate() as ComponentBaseView;
                view.InitializeComponent(slotDataSets, draft.widthInTiles, draft.heightInTiles);
                return view;
            }
            catch (Exception ex)
            {
                CustomLogger.PrintErr($"ComponentTemplate is not or not well defined: {draft?.identifier} - Exception: {ex.Message}");
                throw;
            }
        }
    
        public List<int> GetAllComponentIDs()
        {
            return packedComponentCache.Keys.ToList();
        }
    }
}
