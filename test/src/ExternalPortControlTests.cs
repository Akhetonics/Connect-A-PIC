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
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConnectAPIC.test.src {
    public class ExternalPortControlTests : TestClass {
        private readonly ILog _log = new GDLog(nameof(ExternalPortControlTests));

        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public PortsContainerView MyPortsContainerView { get; set; }
        public ControlMenu MyControlMenu { get; set; }
        public ExternalPortView MyRandomExternalPort { get; set; }

        public ExternalPortControlTests(Node testScene) : base(testScene) { }

        [SetupAll]
        public async Task Setup() {
            MyFixture = new Fixture(TestScene.GetTree());
            try {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
                MyPortsContainerView = MyGameManager.GetNode<PortsContainerView>("GridView/PortContainer");

                //control menu is the last child of ports container view (before that are ports)
                MyControlMenu = MyPortsContainerView.GetChild<ControlMenu>(8);

                //there are 9 children including control menu (last) others are ports
                MyRandomExternalPort = MyPortsContainerView.GetChild<ExternalPortView>(new Random().Next(0, 7));
            } catch (Exception ex) {
                _log.Print(ex.Message);
            }
        }

        [Test]
        public async Task Test() {
            // drag view to show full control menu
            TestScene.GetViewport().DragMouse(new Vector2(200, 200), new Vector2(400,200), MouseButton.Right);

            // control menu should connect to MyRandomExternaPort and become visible
            var portPosition = MyRandomExternalPort.GlobalPosition + new Vector2(-50, 25); //some offset so it won't click corner
            await FrameWaitingMoveAndClickMouse(portPosition);
            await TestScene.GetTree().NextFrame();
            bool controlMenuVisible = MyControlMenu.Visible;

            //button group automatically manages radio button behaviour (disabling others when one is pressed)
            //port types according to indexes 0 - red, 1 - green, 2 - blue, 3 - output

            //Test port switching
            var radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + new Vector2(20, 25); //offset to not click corner

            //red input
            if (!MyRandomExternalPort.ViewModel.IsInput
                || MyRandomExternalPort.ViewModel.Color != LaserType.Red
            ) {
                await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            }

            //red input
            bool startedAsInput = MyRandomExternalPort.ViewModel.IsInput;

            //output
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[3].GlobalPosition + new Vector2(20, 25);
            await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            bool switchedToOutput = !MyRandomExternalPort.ViewModel.IsInput;

            //red input again
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + new Vector2(20, 25);
            await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            bool switchedToInput = MyRandomExternalPort.ViewModel.IsInput;

            //test color switching
            // blue
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[2].GlobalPosition + new Vector2(20, 25);
            await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            LaserType changeColorToBlue = MyRandomExternalPort.ViewModel.Color;

            // green
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[1].GlobalPosition + new Vector2(20, 25);
            await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            LaserType changeColorToGreen = MyRandomExternalPort.ViewModel.Color;

            // red
            radioButtonPosition = MyControlMenu.ButtonGroup.GetButtons()[0].GlobalPosition + new Vector2(20, 25);
            await FrameWaitingMoveAndClickMouse(radioButtonPosition);
            LaserType changeColorToRed = MyRandomExternalPort.ViewModel.Color;


            //test slider value changing command
            int randomSliderValue = new Random().Next(-193, -6);
            float approximateValue = -randomSliderValue / 188.0f;

            var sliderPosition = MyControlMenu.GlobalPosition + new Vector2(-193, 90);
            await FrameWaitingMoveAndClickMouse(sliderPosition);
            TestScene.GetViewport().DragMouse(sliderPosition + new Vector2(200, 0), sliderPosition + new Vector2(-randomSliderValue, 0) + new Vector2(200, 0), MouseButton.Left);
            await TestScene.GetTree().NextFrame();

            //assertions
            controlMenuVisible.ShouldBeTrue();
            startedAsInput.ShouldBeTrue();
            switchedToOutput.ShouldBeTrue();
            switchedToInput.ShouldBeTrue();
            changeColorToBlue.ShouldBe<LaserType>(LaserType.Blue);
            changeColorToGreen.ShouldBe<LaserType>(LaserType.Green);
            changeColorToRed.ShouldBe<LaserType>(LaserType.Red);
            MyRandomExternalPort.ViewModel.Power[0].ShouldBeInRange((float)approximateValue - 0.1f, (float)approximateValue + 0.1f);
            MyRandomExternalPort.ViewModel.Power[1].ShouldBeInRange(-0.01f, 0.01f);
            MyRandomExternalPort.ViewModel.Power[2].ShouldBeInRange(-0.01f, 0.01f);
        }

        public async Task FrameWaitingMoveAndClickMouse(Vector2 position, MouseButton mouseButton = MouseButton.Left, int framesAfterMove = 1, int framesAfterClick = 1){
            TestScene.GetViewport().MoveMouseTo(position + new Vector2(200, 0));
            await TestScene.GetTree().NextFrame(framesAfterMove);
            TestScene.GetViewport().ClickMouseAt(position + new Vector2(200, 0), mouseButton);
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
