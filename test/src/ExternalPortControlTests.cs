using CAP_Core.ExternalPorts;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Input;
using Chickensoft.GodotTestDriver.Util;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scenes.RightClickMenu;
using Godot;
using JetBrains.Annotations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConnectAPIC.test.src
{

    [Sequential]
    public class ExternalPortControlTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExternalPortControlTests));

        private static readonly Vector2 SliderKnobLocalPosition = new(-193, 90); // Offset of slider knob from control menu position
        private static readonly float SliderLengthInPixels = 185f;               // Total length of slider in pixels
        private static readonly Vector2 DragPoint = new(200, 200);               // Safe position for mouse to click and drag from
        private static readonly Vector2 LeftDragOffset = new(200, 0);            // How much camera should be dragged left
        private static readonly Vector2 RightDragOffset = new(-1600, 0);         // How much camera should be dragged right
        private static readonly Vector2 LeftPortPositionOffset = new(-50, 25);   // Offset from left external port corner to avoid clicking on the corner
        private static readonly Vector2 RightPortPositionOffset = new(50, 25);   // Offset from right external port corner to avoid clicking on the corner
        private static readonly Vector2 RadioButtonPositionOffset = new(20, 25); // Offset from port mode radio button corner to avoid clicking on the corner
        private static readonly float Epsilon = 0.05f;                           // Small positive value for approximations

        public Fixture MyFixture { get; private set; }
        public GameManager MyGameManager { get; private set; }
        public PortsContainerView MyPortsContainerView { get; private set; }
        public ControlMenu MyControlMenu { get; private set; }
        public List<ExternalPortView> MyLeftExternalPorts { get; private set; }
        public List<ExternalPortView> MyRightExternalPorts { get; private set; }

        public ExternalPortControlTests(Node testScene) : base(testScene) { }

        [SetupAll]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());

            try
            {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
                MyPortsContainerView = MyGameManager.GetNode<PortsContainerView>("GridView/PortContainer");
                MyLeftExternalPorts = new();
                MyRightExternalPorts = new();

                var children = MyPortsContainerView.GetChildren();

                // Control menu is the last child of ports container view
                MyControlMenu = (ControlMenu)children[children.Count - 1];

                // There are 9 children including control menu (last), others are ports
                for (int i = 0; i < children.Count - 1; i++)
                {
                    var externalPort = (ExternalPortView)children[i];
                    if (externalPort.ViewModel.IsLeftPort)
                        MyLeftExternalPorts.Add(externalPort);
                    else
                        MyRightExternalPorts.Add(externalPort);
                }

            }
            catch (Exception ex)
            {
                _log.Print($"Setup failed: {ex.Message}");
                throw; // Rethrow to ensure test setup failures are noticed
            }
        }

        [Test]
        [Timeout(100_000)]
        public async Task Test()
        {
            var results = new List<string>();

            // Drag view to show full control menu
            TestScene.GetViewport().DragMouse(DragPoint, DragPoint + LeftDragOffset, MouseButton.Right);

            for (int i = 0; i < MyLeftExternalPorts.Count; i++)
            {
                var result = await ExternalPortTest(MyLeftExternalPorts[i], i != 0);
                if (!result.Success)
                {
                    results.Add($"Port {i}: {result.Message}");
                }
            }

            // Drag to show right ports
            TestScene.GetViewport().DragMouse(DragPoint, DragPoint + RightDragOffset, MouseButton.Right);
            await TestScene.GetTree().NextFrame(2);

            for (int i = 0; i < MyRightExternalPorts.Count; i++)
            {
                var result = await ExternalPortTest(MyRightExternalPorts[i], i != 0, false);
                if (!result.Success)
                {
                    results.Add($"Port {i}: {result.Message}");
                }
            }

            results.ShouldBeEmpty();
        }

        private async Task<(bool Success, string Message)> ExternalPortTest(ExternalPortView port, bool waitForMenuToMoveToPosition, bool leftPort = true)
        {
            try
            {
                // Control menu should connect to MyRandomExternalPort and become visible
                var portPosition = port.GetNode<Control>("FlipContainer").GlobalPosition + (leftPort ? LeftPortPositionOffset : RightPortPositionOffset);

                await MoveAndClickMouseAndWaitAsync(portPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));

                if (waitForMenuToMoveToPosition)
                {
                    await TestScene.GetTree().Wait((ControlMenu.TRAVEL_TIME * 1.2f));
                }

                await TestScene.GetTree().NextFrame();

                if (!MyControlMenu.Visible)
                {
                    return (false, "Control menu is not visible");
                }

                // Test port switching
                var portSwitchingResult = await TestPortSwitching(port, leftPort);
                if (!portSwitchingResult.Success)
                {
                    return portSwitchingResult;
                }

                // Test color switching
                var colorSwitchingResult = await TestColorSwitching(port, leftPort);
                if (!colorSwitchingResult.Success)
                {
                    return colorSwitchingResult;
                }

                // Test slider value changing command
                var sliderValueChangeResult = await TestSliderValueChange(port, leftPort);
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

        private async Task<(bool Success, string Message)> TestPortSwitching(ExternalPortView port, bool leftPort = true)
        {
            try
            {
                var radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;

                // Red input
                if (!port.ViewModel.IsInput || port.ViewModel.Color != LaserType.Red)
                {
                    await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
                }

                bool startedAsInput = port.ViewModel.IsInput;

                // Output
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[3].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
                bool switchedToOutput = !port.ViewModel.IsInput;

                // Red input again
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
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

        private async Task<(bool Success, string Message)> TestColorSwitching(ExternalPortView port, bool leftPort = true)
        {
            try
            {
                Vector2 radioButtonPosition;

                // Blue
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[2].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
                if (port.ViewModel.Color != LaserType.Blue)
                {
                    return (false, "Failed to switch to Blue");
                }

                // Green
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[1].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
                if (port.ViewModel.Color != LaserType.Green)
                {
                    return (false, "Failed to switch to Green");
                }

                // Red
                radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + RadioButtonPositionOffset;
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset));
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

        private async Task<(bool Success, string Message)> TestSliderValueChange(ExternalPortView port, bool leftPort = true)
        {
            try
            {
                // Offset of 45%
                float newValue = 0.45f * SliderLengthInPixels;
                float approximateValue = newValue / SliderLengthInPixels;
                var sliderPosition = MyControlMenu.GlobalPosition + SliderKnobLocalPosition;
                await MoveAndClickMouseAndWaitAsync(sliderPosition, LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset), framesAfterClick: 5, framesAfterMove: 5);

                float zeroedSliderValue = port.ViewModel.Power.Length();
                sliderPosition += LeftDragOffset + (leftPort ? Vector2.Zero : RightDragOffset);

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

        public async Task MoveAndClickMouseAndWaitAsync(Vector2 position, MouseButton mouseButton = MouseButton.Left, int framesAfterMove = 1, int framesAfterClick = 1){
            await MoveAndClickMouseAndWaitAsync(position, LeftDragOffset, mouseButton, framesAfterMove, framesAfterClick);
        }

        public async Task MoveAndClickMouseAndWaitAsync(Vector2 position, Vector2 offset, MouseButton mouseButton = MouseButton.Left, int framesAfterMove = 1, int framesAfterClick = 1)
        {
            TestScene.GetViewport().MoveMouseTo(position + offset);
            await TestScene.GetTree().NextFrame(framesAfterMove);
            TestScene.GetViewport().ClickMouseAt(position + offset, mouseButton);
            await TestScene.GetTree().NextFrame(framesAfterClick);
        }

        [CleanupAll]
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
