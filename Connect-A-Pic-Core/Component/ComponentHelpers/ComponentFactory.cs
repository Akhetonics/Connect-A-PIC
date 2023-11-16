using AtetDataFormats.OpenEPDA;
using CAP_Core.Helpers;

namespace CAP_Core.Component.ComponentHelpers
{
    public class ComponentFactory
    {
        private List<Component> ComponentDrafts { set; get; } = new List<Component>();
        private static ComponentFactory instance = instance ?? new ComponentFactory();
        public static ComponentFactory Instance
        {
            get
            {
                instance ??= new ComponentFactory();
                return instance;
            }
        }
        public Component CreateComponent(int componentTypeNumber)
        {
            var newComponent = ComponentDrafts.Single(component => component.TypeNumber == componentTypeNumber);
            return (Component)newComponent.Clone();
        }

        public IntVector GetDimensions(int componentTypeNumber)
        {
            var componentDraft = ComponentDrafts.Single(component => component.TypeNumber == componentTypeNumber);
            return new IntVector(componentDraft.WidthInTiles, componentDraft.HeightInTiles);
        }
        public void InitializeComponentDrafts(List<Component> componentDrafts)
        {
            this.ComponentDrafts = componentDrafts;
        }
    }
}