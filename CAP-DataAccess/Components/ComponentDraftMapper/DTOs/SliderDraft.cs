using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CAP_DataAccess.Components.ComponentDraftMapper.DTOs
{
    public enum SliderTypes
    {
        LinearSlider = 0,
    }
    public class SliderDraft
    {
        [JsonPropertyName("sliderNumber")] 
        public int SliderNumber { get; set; } // used to identify this Slider in a formula e.g. SliderNumber = 0 would be accessed using "SLIDER0"
        [JsonPropertyName("godotSliderName")] 
        public string GodotSliderName { get; set; } // used to find the godot slider in the scene
        [JsonPropertyName("godotSliderLabelName")] 
        public string GodotSliderLabelName { get; set; } // used to identify the godot label in the scene
        [JsonPropertyName("minVal")]
        public double MinVal { get; set; } = 0.0;
        [JsonPropertyName("maxVal")]
        public double MaxVal { get; set; } = 1.0;
        [JsonPropertyName("steps")]
        public int Steps { get; set; } = 100;
        [JsonPropertyName("type")]
        public SliderTypes Type { get; set; } = SliderTypes.LinearSlider; // there is only one kind of slider at the moment.
    }
}
