using AtetDataFormats.OpenEPDA;

namespace CAP_Core.Component.ComponentHelpers
{
    public class ComponentFactory
    {
        private static ComponentFactory instance;
        public static List<Component> components = new List<Component>();
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

            return (Component)Activator.CreateInstance(componentTypeNumber);
        }

        public void InitializeComponentDrafts(List<Component> componentDrafts)
        {
            throw new NotImplementedException();
        }
        public Component LoadComponent(string yamlOfSBBComponent)
        {
            SBB sbb = SBBBuilder.createSBB(yamlOfSBBComponent);
            Component newComponent;
            // create Component from sbb
            throw new NotImplementedException();
        }
    }
}