using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Godot.Range;

namespace ConnectAPIC.Scripts.View.ComponentViews
{

    public class SliderManager
    {
        private List<Godot.Slider> Sliders { get; set; } = new();
        public ComponentViewModel ViewModel { get; }
        public TextureRect Parent { get; }

        public const string SliderNumberMetaID = "SliderNumber";
        public const string SliderLabelMetaID = "SliderLabel";
        public const string SliderIsCallbackRegistered = "SliderIsCallbackRegistered";

        public SliderManager(ComponentViewModel viewModel , TextureRect parent)
        {
            ViewModel = viewModel;
            this.Parent = parent;
            ViewModel.SliderData.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var slider in e.NewItems.Cast<SliderViewData>().ToList())
                    {
                        FindAndInitializeSlider(slider);
                    }
                }
            };
            ViewModel.SliderChanged += (int sliderNumber, double newVal) => SetSliderValue(sliderNumber, newVal);
        }

        private void SetSliderValue(int sliderNumber, double value)
        {
            var slider = GetSliderByNumber(sliderNumber);
            var label = (RichTextLabel)slider.GetMeta(SliderLabelMetaID);
            slider.Value = value;
            slider.CallDeferred(nameof(SetSliderLabelText), label, value);
        }

        private Godot.Slider GetSliderByNumber(int sliderNumber)
        {
            return Sliders.Single(s => (int)s.GetMeta(SliderNumberMetaID) == sliderNumber);
        }

        // initialize one of the existing sliders
        private void FindAndInitializeSlider(SliderViewData sliderData)
        {
            var label = (RichTextLabel)Parent.FindChild(sliderData.GodotSliderLabelName, true, false);
            var godotSlider = Parent.FindChild(sliderData.GodotSliderName, true, false) as Godot.Slider;
            godotSlider.MinValue = sliderData.MinVal;
            godotSlider.MaxValue = sliderData.MaxVal;
            godotSlider.SetMeta(SliderNumberMetaID, sliderData.Number);
            godotSlider.SetMeta(SliderLabelMetaID, label);
            godotSlider.SetMeta(SliderIsCallbackRegistered, true);
            godotSlider.ValueChanged += (newVal) =>
            {
                SetSliderLabelText(label, newVal);
                ViewModel.SetSliderValue(sliderData.Number, newVal);
            };
            godotSlider.Value = sliderData.Value;
            SetSliderLabelText(label, sliderData.Value);
            godotSlider.Step = (sliderData.MaxVal - sliderData.MinVal) / sliderData.Steps; // step is the distance between two steps in value
            this.Sliders.Add(godotSlider);
        }

        private void SetSliderLabelText(RichTextLabel label, double newVal) => label.Text = $"[center]{newVal:F2}";
    }
}
