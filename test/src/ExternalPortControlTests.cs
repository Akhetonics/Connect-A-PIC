using CAP_Core.ExternalPorts;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Drivers;
using Chickensoft.GodotTestDriver.Input;
using Chickensoft.GodotTestDriver.Util;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scenes.RightClickMenu;
using ConnectAPIC.Scenes.RightClickMenu.Sections;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using MathNet.Numerics;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConnectAPIC.test.src
{
    public class ExternalPortControlTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExternalPortControlTests));

        private static readonly Vector2 SliderKnobLocalPosition = new(-193, 90); // Offset of slider knob from control menu position
        private static readonly float SliderLengthInPixels = 185f;               // Total length of slider in pixels
        private static readonly Vector2 DragPoint = new(200, 200);               // Safe position for mouse to click and drag from
        private static readonly Vector2 DragOffset = new(200, 0);                // How much camera should be dragged
        private static readonly Vector2 PortPositionOffset = new(-50, 25);       // Offset from external port corner to avoid clicking on the corner
        private static readonly Vector2 RadioButtonPositionOffset = new(20, 25); // Offset from port mode radio button corner to avoid clicking on the corner
        private static readonly float Epsilon = 0.05f;                           // Small positive value for approximations

        public Fixture MyFixture { get; private set; }
        public GameManager MyGameManager { get; private set; }
        public PortsContainerView MyPortsContainerView { get; private set; }
        public ControlMenu MyControlMenu { get; private set; }
        public List<ExternalPortView> MyExternalPorts { get; private set; }

        public ExternalPortControlTests(Node testScene) : base(testScene) { }

        [SetupAll]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());

            try
            {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
                MyPortsContainerView = MyGameManager.GetNode<PortsContainerView>("GridView/PortContainer");
                MyExternalPorts = new List<ExternalPortView>();

                var children = MyPortsContainerView.GetChildren();

                // Control menu is the last child of ports container view
                MyControlMenu = (ControlMenu)children[8];

                // There are 9 children including control menu (last), others are ports
                for (int i = 0; i < children.Count - 1; i++)
                {
                    MyExternalPorts.Add((ExternalPortView)children[i]);
                }

            }
            catch (Exception ex)
            {
                _log.Print($"Setup failed: {ex.Message}");
                throw; // Rethrow to ensure test setup failures are noticed
            }
        }

        [Test]
        public async Task Test()
        {
            var results = new List<string>();

            // Drag view to show full control menu
            TestScene.GetViewport().DragMouse(DragPoint, DragPoint + DragOffset, MouseButton.Right);

            for (int i = 0; i < MyExternalPorts.Count; i++)
            {
                var result = await ExternalPortTest(MyExternalPorts[i], i != 0);
                if (!result.Success)
                {
                    results.Add($"Port {i}: {result.Message}");
                }
            }

            results.ShouldBeEmpty();
        }

        private async Task<(bool Success, string Message)> ExternalPortTest(ExternalPortView port, bool waitForMenuToMoveToPosition)
        {
            try
            {
                // Control menu should connect to MyRandomExternalPort and become visible
                var portPosition = port.GlobalPosition + PortPositionOffset;
                await MoveAndClickMouseAndWaitAsync(portPosition);

                if (waitForMenuToMoveToPosition)
                {
                    await TestScene.GetTree().Wait((ControlMenu.TRAVEL_TIME*1.2f));
                }

                await TestScene.GetTree().NextFrame();

                if (!MyControlMenu.Visible)
                {
                    return (false, "Control menu is not visible");
                }

                // Test port switching
                var portSwitchingResult = await TestPortSwitching(port);
                if (!portSwitchingResult.Success)
                {
                    return portSwitchingResult;
                }

                // Test color switching
                var colorSwitchingResult = await TestColorSwitching(port);
                if (!colorSwitchingResult.Success)
                {
                    return colorSwitchingResult;
                }

                // Test slider value changing command
                var sliderValueChangeResult = await TestSliderValueChange(port);
                if (!sliderValueChangeResult.Success)
                {
                    return sliderValueChangeResult;
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> TestPortSwitching(ExternalPortView port)
        {
            try
            {
                var radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;

                // Red input
                if (!port.ViewModel.IsInput || port.ViewModel.Color != LaserType.Red)
                {
                    await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                }

                bool startedAsInput = port.ViewModel.IsInput;

                // Output
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[3].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                bool switchedToOutput = !port.ViewModel.IsInput;

                // Red input again
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                bool switchedToInput = port.ViewModel.IsInput;

                // Assertions
                startedAsInput.ShouldBeTrue();
                switchedToOutput.ShouldBeTrue();
                switchedToInput.ShouldBeTrue();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Port switching exception: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> TestColorSwitching(ExternalPortView port)
        {
            try
            {
                Vector2 radioButtonPosition;

                // Blue
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[2].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                if (port.ViewModel.Color != LaserType.Blue)
                {
                    return (false, "Failed to switch to Blue");
                }

                // Green
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[1].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                if (port.ViewModel.Color != LaserType.Green)
                {
                    return (false, "Failed to switch to Green");
                }

                // Red
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
                if (port.ViewModel.Color != LaserType.Red)
                {
                    return (false, "Failed to switch to Red");
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Color switching exception: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> TestSliderValueChange(ExternalPortView port)
        {
            try
            {
                // Offset of 45%
                float newValue = 0.45f * SliderLengthInPixels;
                float approximateValue = newValue / SliderLengthInPixels;
                var sliderPosition = MyControlMenu.GlobalPosition + SliderKnobLocalPosition;
                await MoveAndClickMouseAndWaitAsync(sliderPosition, framesAfterClick: 5, framesAfterMove: 5);

                float zeroedSliderValue = port.ViewModel.Power.Length();
                sliderPosition += DragOffset;

                TestScene.GetViewport().DragMouse(sliderPosition, sliderPosition + new Vector2(newValue, 0), MouseButton.Left);
                await TestScene.GetTree().NextFrame();

                // Assertions
                zeroedSliderValue.ShouldBeInRange(-Epsilon, Epsilon);
                port.ViewModel.Power[0].ShouldBeInRange(approximateValue - Epsilon, approximateValue + Epsilon);
                port.ViewModel.Power[1].ShouldBeInRange(-Epsilon, Epsilon);
                port.ViewModel.Power[2].ShouldBeInRange(-Epsilon, Epsilon);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Slider value change exception: {ex.Message}");
            }
        }

        public async Task MoveAndClickMouseAndWaitAsync(Vector2 position, MouseButton mouseButton = MouseButton.Left, int framesAfterMove = 1, int framesAfterClick = 1)
        {
            TestScene.GetViewport().MoveMouseTo(position + DragOffset);
            await TestScene.GetTree().NextFrame(framesAfterMove);
            TestScene.GetViewport().ClickMouseAt(position + DragOffset, mouseButton);
            await TestScene.GetTree().NextFrame(framesAfterClick);
        }

        [Cleanup]
        public void Cleanup()
        {
            MyControlMenu.Free();
            MyPortsContainerView.Free();
            MyGameManager.Free();
            MyFixture.Cleanup();
        }

        [Failure]
        public void Failure()
        {
            _log.Print("Something might have gone wrong");
        }
    }
}
