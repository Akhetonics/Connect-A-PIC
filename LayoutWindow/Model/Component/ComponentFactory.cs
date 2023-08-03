using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.Model.Component
{
    public class ComponentFactory
    {
        private static ComponentFactory instance;
        public static ComponentFactory Instance
        {
            get
            {
                instance ??= new ComponentFactory();
                return instance;
            }
        }
        public ComponentBase CreateComponent(Type T)
        {
            if (T == typeof(StraightWaveGuide))
            {
                var item = new StraightWaveGuide();
                return item;
            }
            return null;
        }
    }
}