using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ComponentViews
{
    public class SliderViewData
    {
        public string GodotSliderLabelName { get; set; }
        public string GodotSliderName { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }
        public double InitialValue { get; set; }
        public double Steps { get; set; }
        public int SliderNumber { get; set; }
        public SliderViewData()
        {
            
        }
        public SliderViewData(string godotSliderLabelName, string godotSliderName, double minVal, double maxVal, double initialValue, double steps, int sliderNumber)
        {
            GodotSliderLabelName = godotSliderLabelName;
            GodotSliderName = godotSliderName;
            MinVal = minVal;
            MaxVal = maxVal;
            InitialValue = initialValue;
            Steps = steps;
            SliderNumber = sliderNumber;
        }
    }
}
