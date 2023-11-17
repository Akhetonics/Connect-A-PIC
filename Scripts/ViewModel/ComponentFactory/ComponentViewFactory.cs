using Components.ComponentDraftMapper.DTOs;
using Godot;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class ComponentViewFactory : Node
    {
        [Signal] public delegate void InitializedEventHandler();
        [Export] private Script ComponentBaseScriptPath;
        private List<PackedScene> PackedComponentScenes;
        private Dictionary<int, ComponentSceneAndDraft> PackedComponentCache = new();

        public override void _Ready()
        {
            if(ComponentBaseScriptPath == null)
            {
                CustomLogger.inst.PrintErr($"{nameof(ComponentBaseScriptPath)} has not been attached to ComponentviewFactory in the Godot Editor.");
            }
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
                    if(packedScene == null)
                    {
                        throw new ArgumentException(componentDraft.sceneResPath);
                    }
                } catch( Exception ex)
                {
                    CustomLogger.inst.PrintErr($"Error Loading PackedScene '{componentDraft?.sceneResPath}' of Compopnent: {componentDraft?.identifier} ex: {ex.Message} )");
                    continue;
                }
                packedComponentScenes.Add(componentNumber, new ComponentSceneAndDraft()
                {
                    Draft = componentDraft,
                    Scene = packedScene
                });
                componentNumber++;
            }
            PackedComponentCache = packedComponentScenes;
            EmitSignal(nameof(InitializedEventHandler).Replace("EventHandler", ""));
        }

        public ComponentView CreateComponentView(int componentNR)
        {
            if (!PackedComponentCache.ContainsKey(componentNR))
            {
                CustomLogger.inst.PrintErr("Key does not exist in ComponentCache of ComponentviewFactory: " + componentNR);
            }
            var draft = PackedComponentCache[componentNR].Draft;
            var packedScene = PackedComponentCache[componentNR].Scene;
            var slotDataSets = new List<AnimationSlotOverlayData>();

            try
            {
                foreach (Overlay overlay in draft.overlays)
                {
                    var overlayBluePrint = ResourceLoader.Load<Texture2D>(overlay.overlayAnimTexturePath);
                    if(overlayBluePrint == null)
                    {
                        CustomLogger.inst.PrintErr("BluePrint could not be loaded in Type: " + draft.identifier +" ComponentTypeNR: " + componentNR + " path: " + overlay.overlayAnimTexturePath);
                        continue;
                    }
                    slotDataSets.Add(new AnimationSlotOverlayData()
                    {
                        LightFlowOverlay = overlayBluePrint,
                        OffsetX = overlay.tileOffsetX,
                        OffsetY = overlay.tileOffsetY,
                        Side = overlay.rectSide
                    });
                }
                ComponentView componentView = new();
                componentView.AddChild((TextureRect)packedScene.Instantiate());
                componentView.InitializeComponent(componentNR, slotDataSets, draft.widthInTiles, draft.heightInTiles);
                return componentView;
            }
            catch (Exception ex)
            {
                CustomLogger.inst.PrintErr($"ComponentTemplate is not or not well defined: {draft?.identifier} - Exception: {ex.Message}");
                throw;
            }
        }
    
        public Vector2I GetComponentDimensions(int componentTypeNumber)
        {
            if(PackedComponentCache.TryGetValue(componentTypeNumber, out var component))
            {
                return new Vector2I(component.Draft.widthInTiles, component.Draft.heightInTiles);
            }
            CustomLogger.inst.PrintErr($"ComponentTypeNumber {componentTypeNumber} does not exist in ComponentViewFactory");
            throw new KeyNotFoundException( $"ComponentTypeNumber {componentTypeNumber} does not exist in ComponentViewFactory");
        }
        public List<int> GetAllComponentIDs()
        {
            return PackedComponentCache.Keys.ToList();
        }
    }
}
