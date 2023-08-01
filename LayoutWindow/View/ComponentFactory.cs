using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public partial class ComponentFactory : Node
    {
        private List<ComponentBaseView> AllComponentBlueprints;
        private static ComponentFactory instance;
        public static ComponentFactory Instance
        {
            get { return instance; }
        }

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                QueueFree(); // delete this object as there is already another GameManager in the scene
                return;
            }
            AllComponentBlueprints = GetAllComponentBaseNodes();
        }

        public List<ComponentBaseView> GetAllComponentBaseNodes()
        {
            List<ComponentBaseView> resultList = new ();
            foreach (Node child in GetChildren())
            {
                if (child is ComponentBaseView componentBase)
                {
                    resultList.Add(componentBase);
                }
            }
            return resultList;
        }
        public ComponentBaseView CreateComponent(Type T)
        {
            foreach (ComponentBaseView component in AllComponentBlueprints)
            {
                if (T == component.GetType())
                {
                    var item = component.Duplicate();
                    item._Ready();
                    return item;
                }
            }
            return null;
        }
    }
}