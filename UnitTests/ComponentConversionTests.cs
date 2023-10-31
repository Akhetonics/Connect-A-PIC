using CAP_Core.Component;
using CAP_Core.Component.ComponentHelpers;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;

namespace UnitTests
{
    public class ComponentConversionTests
    {
        [Fact]
        public void TestComponentConversions()
        {
            // get all ComponentBase deriving classes from Model
            var ComponentBaseTypes = typeof(ComponentBase).Assembly.GetTypes().Where(t=>typeof(ComponentBase).IsAssignableFrom(t) ).ToList();
            // get all ComponentBaseView deriving Classes from View
            var ComponentBaseViewTypes = typeof(ComponentBaseView).Assembly.GetTypes().Where(t => typeof(ComponentBaseView).IsAssignableFrom(t)).ToList();
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

            Assert.True(ComponentViewModelTypeConverter.ToModel(typeof(StraightWaveGuideView)) == typeof(StraightWaveGuide));
            Assert.True(ComponentViewModelTypeConverter.ToModel(typeof(ComponentBaseView)) == typeof(ComponentBase));
            Assert.True(ComponentViewModelTypeConverter.ToModel(typeof(GratingCouplerView)) == typeof(GratingCoupler));
            Assert.Empty(Errors);
            // try to convert all ComponentBaseView classes to Model

        }
       
    }
}