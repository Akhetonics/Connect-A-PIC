using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class ComponentBaseViewModel
    {
        public ComponentBaseView ComponentBaseView { get; private set; }
        public ComponentBase ComponentBase { get; private set; }
        public ComponentBaseViewModel(ComponentBaseView componentView, ComponentBase component)
        {
            this.ComponentBaseView = componentView;
            this.ComponentBase = component;
        }
        
    }
}
