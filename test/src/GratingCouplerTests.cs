using CAP_Core.Components;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using Shouldly;
using System.Threading.Tasks;
using System.Linq;
using ConnectAPIC.LayoutWindow.View;
using System.Collections.Generic;
using CAP_Core.ExternalPorts;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;

namespace ConnectAPIC.test.src
{
    public class GratingCouplerTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(GratingCouplerTests));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public GratingCouplerTests(Node testScene) : base(testScene) { }
        public ComponentView GratingCoupler { get; set; }
        [Setup]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());
            MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
            // find proper tool from component factory
            int gratingComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "GratingCoupler").Key;
            // instantiate tool at the height of the laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPortManager.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;

            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(gratingComponentNr, 0, firstInputTileY, DiscreteRotation.R0));
            GratingCoupler = MyGameManager.GridView.GridComponentViews[0, firstInputTileY];

        }
        [Test]
        public async Task TestLightVectorAssignment()
        {
            var outflowSide = CAP_Core.Tiles.RectSide.Up;
            var inflowSide = CAP_Core.Tiles.RectSide.Left;
            var redLaser = LaserType.Red;
            var upperLightVector = new LightAtPin(0, 0, outflowSide, redLaser, 0, 1);
            var lightAtPins = new List<LightAtPin>() {
                new (0,0,inflowSide,redLaser,1,0),
                upperLightVector,
            };
            GratingCoupler.ShouldNotBeNull("the grating coupler should have been loaded successfully but it didn't");
            GratingCoupler.ViewModel.DisplayLightVector(lightAtPins);
            await MyGameManager.GridView.ShowLightPropagation();
            await TestScene.GetTree().NextFrame(10);
            var inflowShaderParam = (Vector4)(GratingCoupler.OverlayManager.AnimationSlots[0].BaseOverlaySprite.Material as ShaderMaterial).GetShaderParameter(ShaderParameterNames.LightInFlow + 1);
            inflowShaderParam.X.ShouldBe(1, "because we have set the light input to be 1 for red laser ");
        }

        [Cleanup]
        public void Cleanup()
        {
            MyGameManager.Free();
            MyFixture.Cleanup();
        }

        [Failure]
        public void Failure() =>
          _log.Print("something might have gone wrong");
    }
}
