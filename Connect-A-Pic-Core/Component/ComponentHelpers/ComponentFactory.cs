using AtetDataFormats.OpenEPDA;

namespace CAP_Core.Component.ComponentHelpers
{
    public class ComponentFactory
    {
        private List<Component> ComponentDrafts = new List<Component>();
        private static ComponentFactory instance;
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

        public void InitializeComponentDrafts(List<Component> componentDrafts)
        {
            this.ComponentDrafts = componentDrafts;
        }
    }
}