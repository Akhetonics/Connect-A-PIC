using CAP_Core.Component;
using CAP_Core.Component.ComponentHelpers;
using ConnectAPIC.LayoutWindow.View;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public static class ComponentViewModelTypeConverter
    {
        private static readonly Dictionary<Type, Type> ModelViewTable = new()
        {
            { typeof(ComponentBase), typeof(ComponentBaseView) },
            { typeof(StraightWaveGuide), typeof(StraightWaveGuideView) },
            { typeof(GratingCoupler), typeof(GratingCouplerView) },
            { typeof(DirectionalCoupler), typeof(DirectionalCouplerView) },
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
