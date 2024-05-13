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

namespace ConnectAPIC.test.src {
    public class ExternalPortControlTests : TestClass {
        private readonly ILog _log = new GDLog(nameof(ExternalPortControlTests));

        private static Vector2 sliderKnobLocalPosition = new(-193, 90); // offset of slider knob from control menu position
        private static float sliderLengthInPixels = 185f;               // total Length of slider in pixel
        private static Vector2 dragPoint = new(200, 200);               // safe(where no other component is clicked) position for mouse to click and drag from
        private static Vector2 dragOffset = new(200, 0);                // how much camera should be dragged
        private static Vector2 portPositionOffset = new(-50, 25);       // offset from external port corner so that mouse doesn't click on corner
        private static Vector2 radioButtonPositionOffset = new(20, 25); // offset from port mode radio button corner so that mouse doesn't click on corner
        private static float epsilon = 0.05f;                           // some small positive value for approximations

        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public PortsContainerView MyPortsContainerView { get; set; }
        public ControlMenu MyControlMenu { get; set; }
        public List<ExternalPortView> MyExternalPorts { get; set; }

        public ExternalPortControlTests(Node testScene) : base(testScene) { }

        [SetupAll]
        public async Task Setup() {
            MyFixture = new Fixture(TestScene.GetTree());
            try {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
                MyPortsContainerView = MyGameManager.GetNode<PortsContainerView>("GridView/PortContainer");
                MyExternalPorts = new List<ExternalPortView>();
                var children = MyPortsContainerView.GetChildren();

                //control menu is the last child of ports container view (before that are ports)
                MyControlMenu = (ControlMenu)children[8];

                //there are 9 children including control menu (last) others are ports
                for (int  i = 0; i < children.Count - 1; i++)
                    MyExternalPorts.Add((ExternalPortView)children[i]);

            } catch (Exception ex) {
                _log.Print(ex.Message);
            }
        }

        [Test]
        public async Task Test() {
            // drag view to show full control menu
            TestScene.GetViewport().DragMouse(dragPoint, dragPoint + dragOffset, MouseButton.Right);
            await ExternalPortTest(MyExternalPorts[0]);
            await TestScene.GetTree().NextFrame();
            await ExternalPortTest(MyExternalPorts[2], true);
            await TestScene.GetTree().NextFrame();
            await ExternalPortTest(MyExternalPorts[3], true);
            await TestScene.GetTree().NextFrame();
            await ExternalPortTest(MyExternalPorts[7], true);
        }

        private async Task ExternalPortTest(ExternalPortView port, bool waitForMenuToMoveToPosition = false) {
            // control menu should connect to MyRandomExternalPort and become visible
            var portPosition = port.GlobalPosition + portPositionOffset; await MoveAndClickMouseAndWaitAsync(portPosition);
            if (waitForMenuToMoveToPosition) {
                await Task.Delay((int)(ControlMenu.TRAVEL_TIME * 1000));
            }
            await TestScene.GetTree().NextFrame();
            bool controlMenuVisible = MyControlMenu.Visible;

            //button group automatically manages radio button behaviour (disabling others when one is pressed)
            //port types according to indexes 0 - red, 1 - green, 2 - blue, 3 - output

            //Test port switching
            var radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + radioButtonPositionOffset;

            //red input
            if (!port.ViewModel.IsInput
                || port.ViewModel.Color != LaserType.Red
            ) {
                await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            }

            //red input
            bool startedAsInput = port.ViewModel.IsInput;

            //output
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[3].GlobalPosition + radioButtonPositionOffset;
            await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            bool switchedToOutput = !port.ViewModel.IsInput;

            //red input again
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + radioButtonPositionOffset;
            await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            bool switchedToInput = port.ViewModel.IsInput;

            //test color switching
            // blue
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[2].GlobalPosition + radioButtonPositionOffset;
            await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            LaserType changeColorToBlue = port.ViewModel.Color;

            // green
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[1].GlobalPosition + radioButtonPositionOffset;
            await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            LaserType changeColorToGreen = port.ViewModel.Color;

            // red
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + radioButtonPositionOffset;
            await MoveAndClickMouseAndWaitAsync(radioButtonPosition);
            LaserType changeColorToRed = port.ViewModel.Color;


            //test slider value changing command
            //offset of 45%
            float newValue = 0.45f * sliderLengthInPixels;
            float approximateValue = newValue / sliderLengthInPixels;
            var sliderPosition = MyControlMenu.GlobalPosition + sliderKnobLocalPosition;
            await MoveAndClickMouseAndWaitAsync(sliderPosition);
            float zeroedSliderValue = port.ViewModel.Power.Length();
            sliderPosition += dragOffset;
            TestScene.GetViewport().DragMouse(sliderPosition, sliderPosition + new Vector2(newValue, 0), MouseButton.Left);
            await TestScene.GetTree().NextFrame();

            //assertions
            controlMenuVisible.ShouldBeTrue();
            startedAsInput.ShouldBeTrue();
            switchedToOutput.ShouldBeTrue();
            switchedToInput.ShouldBeTrue();
            changeColorToBlue.ShouldBe<LaserType>(LaserType.Blue);
            changeColorToGreen.ShouldBe<LaserType>(LaserType.Green);
            changeColorToRed.ShouldBe<LaserType>(LaserType.Red);
            zeroedSliderValue.ShouldBeInRange(-epsilon, epsilon);
            port.ViewModel.Power[0].ShouldBeInRange(approximateValue - epsilon, approximateValue + epsilon);
            port.ViewModel.Power[1].ShouldBeInRange(-epsilon, epsilon);
            port.ViewModel.Power[2].ShouldBeInRange(-epsilon, epsilon);
        }

        public async Task MoveAndClickMouseAndWaitAsync(Vector2 position, MouseButton mouseButton = MouseButton.Left, int framesAfterMove = 1, int framesAfterClick = 1){
            TestScene.GetViewport().MoveMouseTo(position + dragOffset);
            await TestScene.GetTree().NextFrame(framesAfterMove);
            TestScene.GetViewport().ClickMouseAt(position + dragOffset, mouseButton);
            await TestScene.GetTree().NextFrame(framesAfterClick);
        }

        [Cleanup]
        public void Cleanup() {
            MyControlMenu.Free();
            MyPortsContainerView.Free();
            MyGameManager.Free();
            MyFixture.Cleanup();
        }

        [Failure]
        public void Failure() =>
          _log.Print("something might have gone wrong");
    }
}
