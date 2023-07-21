using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public partial class ComponentFactory : Node
    {
        private List<ComponentBase> AllComponentBlueprints;
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

        public List<ComponentBase> GetAllComponentBaseNodes()
        {
            List<ComponentBase> resultList = new List<ComponentBase>();
            foreach (Node child in GetChildren())
            {
                if (child is ComponentBase componentBase)
                {
                    resultList.Add(componentBase);
                }
            }
            return resultList;
        }
        public ComponentBase CreateComponent(Type T)
        {
            foreach (ComponentBase component in AllComponentBlueprints)
            {
                if (T == component.GetType())
                {
                    return component.Duplicate();
                }
            }
            return null;
        }
    }
}