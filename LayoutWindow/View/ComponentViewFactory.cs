using ConnectAPIC.LayoutWindow.Model.Component;
using ConnectAPIC.LayoutWindow.View;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scenes.Component
{
    public partial class ComponentViewFactory : Node
    {
        private List<ComponentBaseView> AllComponentViewBlueprints;
        private static ComponentViewFactory instance { get; set; }
        public static ComponentViewFactory Instance
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
            AllComponentViewBlueprints = GetAllComponentBaseViewNodes();
        }

        public List<ComponentBaseView> GetAllComponentBaseViewNodes()
        {
            List<ComponentBaseView> resultList = new List<ComponentBaseView>();
            foreach (Node child in GetChildren())
            {
                if (child is ComponentBaseView componentBase)
                {
                    resultList.Add(componentBase);
                }
            }
            return resultList;
        }
        public ComponentBaseView CreateComponentView(Type ViewTypeListedInFactoryChildren)
        {
            if (ViewTypeListedInFactoryChildren.IsAssignableFrom(typeof(ComponentBaseView))){
                
            }
            foreach (ComponentBaseView component in AllComponentViewBlueprints)
            {
                if (ViewTypeListedInFactoryChildren == component.GetType())
                {
                    var item = component.Duplicate();
                    item._Ready();
                    return item;
                }
            }
            throw new ComponentTemplateMissingException(ViewTypeListedInFactoryChildren);
        }
    }
}