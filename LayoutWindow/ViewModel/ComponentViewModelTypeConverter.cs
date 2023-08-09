using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public static class ComponentViewModelTypeConverter
    {
        public static Type ToModel(Type ComponentViewType) {
            if (typeof(StraightWaveGuideView).IsAssignableFrom(ComponentViewType))
            {
                return typeof(StraightWaveGuide);
            }
            return null;
        }
        public static Type ToView(Type ComponentType)
        {
            if (ComponentType == typeof(StraightWaveGuide))
            {
                return typeof(StraightWaveGuideView);
            }
            return null;
        }

    }
}
