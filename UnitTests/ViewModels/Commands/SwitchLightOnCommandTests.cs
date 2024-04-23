using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using Moq;

namespace UnitTests.ViewModels.Commands
{
    public class SwitchLightOnCommandTests
    {
        private readonly LightManager lightManager;
        private readonly SwitchOnLightCommand command;

        public SwitchLightOnCommandTests()
        {
            lightManager = new LightManager();
            command = new SwitchOnLightCommand(lightManager);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotBoolean()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public async Task TestExecute()
        {
            // arrange
            bool oldLightStatus = lightManager.IsLightOn;

            // act
            await command.ExecuteAsync(!oldLightStatus);

            bool newLightStatus = lightManager.IsLightOn;

            command.Undo();

            // assert
            Assert.True(newLightStatus != oldLightStatus);
            Assert.True(lightManager.IsLightOn == oldLightStatus);
        }
    }

}
