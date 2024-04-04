using CAP_Core.Components;
using CAP_Core.Helpers;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ComponentViews;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using SuperNodes.Types;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ConnectAPIC.LayoutWindow.View
{
    [SuperNode(typeof(Dependent))]
    public partial class ComponentView : TextureRect
    {
        public override partial void _Notification(int what);
        [Dependency] public GridViewModel GridViewModel => DependOn<GridViewModel>();
        public int WidthInTiles { get; set; }
        public int HeightInTiles { get; set; }
        private Node2D RotationArea { get; set; } // the part of the component that rotates
        public Sprite2D OverlayBluePrint { get; set; }
        public ComponentViewModel ViewModel { get; private set; }
        public new float RotationDegrees
        {
            get => RotationArea?.RotationDegrees ?? 0;
            set
            {
                if (RotationArea?.RotationDegrees != null)
                    RotationArea.RotationDegrees = value;
            }
        }
        private new float Rotation { get => RotationArea.Rotation; set => RotationArea.Rotation = value; }
        public SliderManager SliderManager { get; private set; }
        public OverlayManager OverlayManager { get; set; }

        public void OnResolved()
        {
            GridViewModel.SelectionGroupManager.SelectedComponents.CollectionChanged += SelectedComponents_CollectionChanged;
        }

        private void SelectedComponents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Check this component was added
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action==NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems.Contains(new IntVector(ViewModel.GridX, ViewModel.GridY)))
                {
                    Modulate = new Godot.Color(0, 1, 0);
                }
            }
            // Check if items were removed
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Contains(new IntVector(ViewModel.GridX, ViewModel.GridY)))
                {
                    Modulate = new Godot.Color(1, 1, 1);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Modulate = new Godot.Color(1, 1, 1);
            }

        }

        public ComponentView()
        {
            ViewModel = new ComponentViewModel();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            SliderManager = new SliderManager(ViewModel, this);
            OverlayManager = new OverlayManager(this);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComponentViewModel.RotationCC))
            {
                OverlayManager.AnimationSlots?.ForEach(a => a.RotateAttachedComponentCC(ViewModel.RotationCC)); // the slots need to know the rotation for proper animation matching
                RotationDegrees = ViewModel.RotationCC.ToDegreesClockwise();
            }
            else if (e.PropertyName == nameof(ComponentViewModel.LightsAtPins))
            {
                var lightAndSlots = new List<(LightAtPin light, List<AnimationSlot>)>();
                var shaderSlotNumber = 1;

                foreach (LightAtPin light in ViewModel.LightsAtPins)
                {
                    var slots = AnimationSlot.FindMatching(OverlayManager.AnimationSlots, light);
                    foreach (var slot in slots)
                    {
                        OverlayManager.ShowAndAssignInAndOutFlowShaderData(slot, light, shaderSlotNumber);
                        shaderSlotNumber++;
                    }
                }
            }
            else if (e.PropertyName == nameof(ComponentViewModel.Position))
            {
                Position = new Godot.Vector2(ViewModel.Position.X, ViewModel.Position.Y);
            }
            else if (e.PropertyName == nameof(ComponentViewModel.Visible))
            {
                Visible = ViewModel.Visible;
            }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public void Initialize(List<AnimationSlotOverlayData> animationSlotOverlays, int widthInTiles, int heightInTiles)
        {
            RotationArea = (FindChild("?otation*", true, false) ?? FindChild("ROTATION*", true, false)) as Node2D;
            this.WidthInTiles = widthInTiles;
            this.HeightInTiles = heightInTiles;
            OverlayManager.Initialize(animationSlotOverlays, WidthInTiles, HeightInTiles);
        }

        public void HideLightVector() => OverlayManager.HideLightVector();

        public virtual ComponentView Duplicate()
        {
            var copy = (ComponentView)base.Duplicate();
            copy.Initialize(OverlayManager.AnimationSlotRawData, WidthInTiles, HeightInTiles);
            copy._Ready();
            copy.ViewModel = new ComponentViewModel();
            copy.ViewModel.RotationCC = ViewModel.RotationCC; // give the new copy the proper RotationCC so that it has the correct rotation

            // deep copy that list of sliders
            List<SliderViewData> newSliderData = SliderManager.DuplicateSliders();
            copy.ViewModel.InitializeComponent(ViewModel.TypeNumber, newSliderData, ViewModel.Logger);
            return copy;
        }
        public override void _ExitTree()
        {
            this.GridViewModel.SelectionGroupManager.SelectedComponents.CollectionChanged -= SelectedComponents_CollectionChanged;
            ViewModel.TreeExited();
            base._ExitTree();
        }
    }
}
