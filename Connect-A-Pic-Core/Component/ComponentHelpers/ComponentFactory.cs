using AtetDataFormats.OpenEPDA;

namespace CAP_Core.Component.ComponentHelpers
{
    public class ComponentFactory
    {
        private static ComponentFactory instance;
        public static List<ComponentBase> components = new List<ComponentBase>();
        public static ComponentFactory Instance
        {
            get
            {
                instance ??= new ComponentFactory();
                return instance;
            }
        }
        public ComponentBase CreateComponent(int componentTypeNumber)
        {

            return (ComponentBase)Activator.CreateInstance(componentTypeNumber);
        }

        public void InitializeComponentDrafts(List<ComponentBase> componentDrafts)
        {
            throw new NotImplementedException();
        }
        public ComponentBase LoadComponent(string yamlOfSBBComponent)
        {
            SBB sbb = SBBBuilder.createSBB(yamlOfSBBComponent);
            ComponentBase newComponent;
            // create Component from sbb
            throw new NotImplementedException();
        }
    }
}