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
        public TextureRect GodotObject { get; }

        public const string SliderNumberMetaID = "SliderNumber";
        public const string SliderLabelMetaID = "SliderLabel";
        public const string SliderIsCallbackRegistered = "SliderIsCallbackRegistered";

        public SliderManager(ComponentViewModel viewModel , TextureRect godotObject)
        {
            ViewModel = viewModel;
            this.GodotObject = godotObject;
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

        public List<SliderViewData> DuplicateSliders()
        {
            var newSliderData = new List<SliderViewData>();
            foreach (var slider in ViewModel.SliderData)
            {
                newSliderData.Add(new SliderViewData()
                {
                    GodotSliderLabelName = slider.GodotSliderLabelName,
                    GodotSliderName = slider.GodotSliderName,
                    Value = slider.Value,
                    MaxVal = slider.MaxVal,
                    MinVal = slider.MinVal,
                    Number = slider.Number,
                    Steps = slider.Steps
                });
            }

            return newSliderData;
        }
        private void SetSliderValue(int sliderNumber, double value)
        {
            var slider = GetSliderByNumber(sliderNumber);
            var label = (RichTextLabel)slider.GetMeta(SliderLabelMetaID);
            slider.Value = value;
            SetSliderLabelText(label, value);
        }

        private Godot.Slider GetSliderByNumber(int sliderNumber)
        {
            return Sliders.Single(s => (int)s.GetMeta(SliderNumberMetaID) == sliderNumber);
        }

        // initialize one of the existing sliders
        private void FindAndInitializeSlider(SliderViewData sliderData)
        {
            var label = GodotObject.FindChild(sliderData.GodotSliderLabelName, true, false) as RichTextLabel;
            var godotSlider = GodotObject.FindChild(sliderData.GodotSliderName, true, false) as Godot.Slider;
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
