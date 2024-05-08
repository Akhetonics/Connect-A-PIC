using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
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
        private List<Godot.Slider> GodotSliders { get; set; } = new();
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
            ViewModel.SliderModelChanged += SetSliderValue;
        }

        private void RemoveSlider(SliderViewData slider)
        {
            throw new NotImplementedException();
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
            return GodotSliders.Single(s =>
            {
                if (GodotObject.IsInstanceValid(s) == false)
                {
                    return false;
                }
                return (int)s.GetMeta(SliderNumberMetaID) == sliderNumber;
            });
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
                ViewModel.ChangeSliderValueThroughTimer(sliderData.Number, newVal);
            };
            sliderData.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if(e.PropertyName == nameof(SliderViewData.Value))
                {
                    SetSliderValue(sliderData.Number, ((SliderViewData)sender).Value);
                }
            };
            godotSlider.Value = sliderData.Value;
            SetSliderLabelText(label, sliderData.Value);
            godotSlider.Step = (sliderData.MaxVal - sliderData.MinVal) / sliderData.Steps; // step is the distance between two steps in value
            this.GodotSliders.Add(godotSlider);
        }


        private void SetSliderLabelText(RichTextLabel label, double newVal) => label.Text = $"[center]{newVal:F2}";
    }
}
