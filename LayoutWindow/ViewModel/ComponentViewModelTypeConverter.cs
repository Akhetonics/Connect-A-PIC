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
        private static readonly Dictionary<Type, Type> ModelViewTable = new()
        {
            { typeof(ComponentBase), typeof(ComponentBaseView) },
            { typeof(StraightWaveGuide), typeof(StraightWaveGuideView) },
            { typeof(GratingCoupler), typeof(GratingCouplerView) },
        };
        public static Type ToModel(Type ComponentViewType) {
            foreach (var modelView in ModelViewTable)
            {
                if (modelView.Value == ComponentViewType)
                {
                    return modelView.Key;
                }
            }
            return null;
        }
        public static Type ToView(Type ComponentType)
        {
            foreach (var modelView in ModelViewTable)
            {
                if (modelView.Key == ComponentType)
                {
                    return modelView.Value;
                }
            }
            return null;
        }

    }
}
