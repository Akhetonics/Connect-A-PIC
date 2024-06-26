using CAP_Contracts.Logger;
using CAP_Core.Grid;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.View.ComponentFactory;
using ConnectAPIC.Scripts.View.ComponentViews;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using MathNet.Numerics;
using SuperNodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{
    [SuperNode(typeof(Dependent))]
    public partial class ComponentViewFactory : Node
    {
        public override partial void _Notification(int what);
        [Dependency] public GridViewModel GridViewModel => DependOn<GridViewModel>();
        [Signal] public delegate void InitializedEventHandler();
        [Export] private Script ComponentBaseScriptPath;
        private List<PackedScene> PackedComponentScenes;
        public Dictionary<int, ComponentSceneAndDraft> PackedComponentCache { get; private set; } = new();
        public ILogger Logger { get; set; }
        public void InitializeComponentDrafts(List<ComponentDraft> drafts, ILogger logger)
        {
            Logger = logger;
            if (ComponentBaseScriptPath == null)
            {
                Logger.PrintErr($"{nameof(ComponentBaseScriptPath)} has not been attached to ComponentViewFactory in the Godot Editor.");
            }
            Dictionary<int, ComponentSceneAndDraft> packedComponentScenes = new();

            int componentNumber = 0;
            // load the PackedScene
            foreach (var componentDraft in drafts)
            {
                PackedScene packedScene;
                try
                {
                    packedScene = GD.Load<PackedScene>(componentDraft.SceneResPath);
                    if (packedScene == null)
                    {
                        throw new ArgumentException(componentDraft.SceneResPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.PrintErr($"Error Loading PackedScene '{componentDraft?.SceneResPath}' of Component: {componentDraft?.Identifier} ex: {ex.Message} )");
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

        public ComponentView CreateComponentView(int componentNR, ComponentViewModel cmpViewModel)
        {
            if (!PackedComponentCache.ContainsKey(componentNR))
            {
                Logger.PrintErr("Key does not exist in ComponentCache of ComponentViewFactory: " + componentNR);
            }
            var draft = PackedComponentCache[componentNR].Draft;
            var packedScene = PackedComponentCache[componentNR].Scene;
            var slotDataSets = MapOverlayModelsToOverlayViews(componentNR, draft);
            try
            {
                ComponentView componentView = new();
                componentView._Ready();
                var actualComponent = (TextureRect)packedScene.Instantiate();
                actualComponent.CustomMinimumSize = new(draft.WidthInTiles * GameManager.TilePixelSize, draft.HeightInTiles * GameManager.TilePixelSize);
                componentView.AddChild(actualComponent);
                componentView.Initialize(slotDataSets, draft.WidthInTiles, draft.HeightInTiles);
                // viewModel has to be added last, so that the componentView has finished constructing before adding the data to initialize sliders etc.
                componentView.SetViewModel(cmpViewModel);
                cmpViewModel.InitializeComponent(componentNR, MapDataAccessSlidersToViewSliders(draft), Logger);
                return componentView;
            }
            catch (Exception ex)
            {
                Logger.PrintErr($"ComponentTemplate is not or not well defined: {draft?.Identifier} - Exception: {ex.Message}");
                throw;
            }
        }

        private List<AnimationSlotOverlayData> MapOverlayModelsToOverlayViews(int componentNR, ComponentDraft draft)
        {
            var slotDataSets = new List<AnimationSlotOverlayData>();
            try
            {
                foreach (Overlay overlay in draft.Overlays)
                {
                    var overlayBluePrint = ResourceLoader.Load<Texture2D>(overlay.OverlayAnimTexturePath);
                    if (overlayBluePrint == null)
                    {
                        Logger.PrintErr($"'{nameof(overlayBluePrint)}' could not be loaded in Type: " + draft.Identifier + " ComponentTypeNR: " + componentNR + " path: " + overlay.OverlayAnimTexturePath);
                        continue;
                    }
                    slotDataSets.Add(
                        new AnimationSlotOverlayData(
                            overlay.OverlayAnimTexturePath,
                            overlay.RectSide,
                            overlay.FlowDirection ?? FlowDirection.Both,
                            overlay.TileOffsetX,
                            overlay.TileOffsetY
                        ));
                }
            }
            catch (Exception ex)
            {
                Logger.PrintErr($"ComponentTemplate is not or not well defined: {draft?.Identifier} - Exception: {ex.Message}");
                throw;
            }
            return slotDataSets;
        }
        private static List<SliderViewData> MapDataAccessSlidersToViewSliders(ComponentDraft draft)
        {
            if (draft.Sliders == null) return new();
            return draft.Sliders.Select(s => new SliderViewData(
                s.GodotSliderLabelName,
                s.GodotSliderName,
                s.MinVal,
                s.MaxVal,
                (s.MaxVal - s.MinVal) / 2,
                s.Steps,
                s.SliderNumber
            )).ToList();
        }

        public Vector2I GetComponentDimensions(int componentTypeNumber)
        {
            if (PackedComponentCache.TryGetValue(componentTypeNumber, out var component))
            {
                return new Vector2I(component.Draft.WidthInTiles, component.Draft.HeightInTiles);
            }
            Logger.PrintErr($"ComponentTypeNumber {componentTypeNumber} does not exist in ComponentViewFactory");
            throw new KeyNotFoundException($"ComponentTypeNumber {componentTypeNumber} does not exist in ComponentViewFactory");
        }
        public List<int> GetAllComponentIDs()
        {
            return PackedComponentCache.Keys.ToList();
        }
    }
}
