using CAP_Core.ExternalPorts;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Drivers;
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
        public ExternalPortView MyRandomExternaPort { get; set; }

        public ExternalPortControlTests(Node testScene) : base(testScene) { }

        [SetupAll]
        public async Task Setup() {
            MyFixture = new Fixture(TestScene.GetTree());
            try {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
                MyPortsContainerView = new PortsContainerView();
                MyPortsContainerView.ExternalPortViewTemplate = ResourceLoader.Load<PackedScene>("res://Scenes/ExternalPorts/external_port.tscn");
                MyPortsContainerView.RightClickMenuTemplate = ResourceLoader.Load<PackedScene>("res://Scenes/RightClickMenu/ControlMenu.tscn");
                MyGameManager.AddChild(MyPortsContainerView);
                //control menu is the last child of ports container view (before that are ports)
                MyControlMenu = MyPortsContainerView.GetChild<ControlMenu>(8);

                //there are 9 children including control menu (last) others are ports
                MyRandomExternaPort = MyPortsContainerView.GetChild<ExternalPortView>(new Random().Next(0, 7));
            } catch (Exception ex) {
                _log.Print(ex.Message);
            }
        }

        [Test]
        public async Task Test() {

            // control menu should connect to MyRandomExternaPort
            MyRandomExternaPort.ViewModel.InvokeClicked();

            //button group automatically manages radio button behaviour (disabling others when one is pressed)
            //port types according to indexes 0 - red, 1 - green, 2 - blue, 3 - output

            //Test port switching
            //red input
            MyControlMenu.ButtonGroup.GetButtons()[0].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.IsInput.ShouldBe<bool>(true);

            //output
            MyControlMenu.ButtonGroup.GetButtons()[3].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.IsInput.ShouldBe<bool>(false);

            //red input
            MyControlMenu.ButtonGroup.GetButtons()[0].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.IsInput.ShouldBe<bool>(true);


            //test color switching
            // blue
            MyControlMenu.ButtonGroup.GetButtons()[2].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.Color.ShouldBe<LaserType>(LaserType.Blue);
            // green
            MyControlMenu.ButtonGroup.GetButtons()[1].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.Color.ShouldBe<LaserType>(LaserType.Green);
            // red
            MyControlMenu.ButtonGroup.GetButtons()[0].ButtonPressed = true;
            MyRandomExternaPort.ViewModel.Color.ShouldBe<LaserType>(LaserType.Red);


            //test slider value chagnding command
            double randomPowerValue = new Random().NextDouble();
            MyControlMenu.ViewModel.InputPowerAdjustCommand.ExecuteAsync(
                new InputPowerAdjustArgs(MyRandomExternaPort.ViewModel.PortModel, randomPowerValue)).Wait();

            //power (vector3) has values in floats and in model its in double but some error is acceptable on view
            MyRandomExternaPort.ViewModel.Power[0].ShouldBeInRange((float)randomPowerValue - 0.01f, (float)randomPowerValue + 0.01f);
            MyRandomExternaPort.ViewModel.Power[1].ShouldBeInRange(0.01f, 0.01f);
            MyRandomExternaPort.ViewModel.Power[2].ShouldBeInRange(0.01f, 0.01f);
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
