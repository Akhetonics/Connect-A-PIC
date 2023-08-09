using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Tiles;

namespace UnitTests
{
    public class ComponentConversionTests
    {
        [Fact]
        public void TestComponentConversions()
        {
            // get all ComponentBase deriving classes from Model
            var ComponentBaseTypes = this.GetType().Assembly.GetTypes().Where(t=>typeof(ComponentBase).IsAssignableFrom(t) ).ToList();
            // get all ComponentBaseView deriving Classes from View
            var ComponentBaseViewTypes = this.GetType().Assembly.GetTypes().Where(t => typeof(ComponentBaseView).IsAssignableFrom(t)).ToList();
            // try to convert all ComponentBase classes to view
            
            List<Type> Errors = new List<Type>();
            foreach(var ComponentBaseType in ComponentBaseTypes)
            {
                var viewType = ComponentViewModelTypeConverter.ToView(ComponentBaseType);
                Console.WriteLine($"Test Conversion of BaseType: {nameof(ComponentBaseType.Name)} - Result:  {viewType?.Name}");
                if (viewType == null)
                {
                    Errors.Add(ComponentBaseType);
                }
            }

            foreach (var CompViewType in ComponentBaseViewTypes)
            {
                var modelType = ComponentViewModelTypeConverter.ToModel(CompViewType);
                Console.WriteLine($"Test Conversion of CompViewType: {nameof(CompViewType.Name)} - Result: {modelType?.Name}");
                if (modelType == null)
                {
                    Errors.Add(CompViewType); 
                }
            }

            Assert.Empty(Errors);
            // try to convert all ComponentBaseView classes to Model

        }
       
    }
}