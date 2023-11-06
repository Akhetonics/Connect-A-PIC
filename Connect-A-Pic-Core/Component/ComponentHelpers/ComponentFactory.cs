using AtetDataFormats.OpenEPDA;

namespace CAP_Core.Component.ComponentHelpers
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
            if (!typeof(ComponentBase).IsAssignableFrom(T))
                throw new ArgumentException($"Type is not of {nameof(ComponentBase)}: {nameof(T) + " " + T.FullName}");

            if (!T.IsClass || T.IsAbstract || T.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"Type is abstract or has no empty constructor: {nameof(T) + " " + T.FullName}");

            return (ComponentBase)Activator.CreateInstance(T);
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