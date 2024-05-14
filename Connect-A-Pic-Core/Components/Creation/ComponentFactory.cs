using CAP_Core.Helpers;

namespace CAP_Core.Components.Creation
{
    public class ComponentFactory : IComponentFactory
    {
        private List<Component> ComponentDrafts { set; get; } = new List<Component>();

        public Component CreateComponent(int componentTypeNumber)
        {
            if (ComponentDrafts.Any(c => c.TypeNumber == componentTypeNumber) == false)
            {
                throw new InvalidOperationException($"componentTypeNumber does not exist in ComponentFactory: {componentTypeNumber}");
            }
            var newComponent = ComponentDrafts.Single(component => component.TypeNumber == componentTypeNumber);
            return (Component)newComponent.Clone();
        }
        public Component CreateComponentByIdentifier(string identifier)
        {
            if(ComponentDrafts == null)
            {
                throw new InvalidOperationException("ComponentDrafts have not yet been loaded from Disk.");
            }
            if (ComponentDrafts.Any(c => c?.Identifier == identifier) == false)
            {
                throw new InvalidOperationException($"Identifier does not exist in ComponentFactory: {identifier}");
            }
            var newComponent = ComponentDrafts.Single(component => component.Identifier == identifier);
            return (Component)newComponent.Clone();
        }

        public IntVector GetDimensions(int componentTypeNumber)
        {
            if (ComponentDrafts.Any(c => c.TypeNumber == componentTypeNumber) == false)
            {
                throw new InvalidOperationException($"ComponentTypeNumber does not exist in ComponentFactory: {componentTypeNumber}");
            }
            var componentDraft = ComponentDrafts.Single(component => component.TypeNumber == componentTypeNumber);
            return new IntVector(componentDraft.WidthInTiles, componentDraft.HeightInTiles);
        }
        public void InitializeComponentDrafts(List<Component> componentDrafts)
        {
            ComponentDrafts = componentDrafts;
        }
    }

    
}
