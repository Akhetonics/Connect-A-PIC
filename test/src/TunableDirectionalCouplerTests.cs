using CAP_Core.ExternalPorts;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using CAP_Core.Components;
using CAP_Core.Tiles;
using Shouldly;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;

namespace ConnectAPIC.test.src
{
    public class TunableDirectionalCouplerTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExampleTest));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public ComponentView TunableDirectionalCoupler { get; set; }
        public LaserType RedLaser { get; set; }
        public TunableDirectionalCouplerTests(Node testScene) : base(testScene) { }
        [Setup]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());
            try
            {
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
            }
            catch (Exception ex)
            {
                _log.Print(ex.Message);
            }

            // find proper component draft from component factory
            int tunableDirectionalCouplerNR = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "DirectionalCoupler").Key;

            // instantiate tool at position attached to laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(tunableDirectionalCouplerNR, 0, firstInputTileY, DiscreteRotation.R0));
            // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
            TunableDirectionalCoupler = MyGameManager.GridViewModel.GridComponentViews[0, firstInputTileY];
            var usedPorts = MyGameManager.Grid.GetUsedExternalInputs();
        }

        [Test]
        public async Task TestShaderAssignment()
        {
            // setup
            // act
            // first test if the connectionWeights are proper
            var component = MyGameManager.Grid.GetComponentAt(TunableDirectionalCoupler.ViewModel.GridX, TunableDirectionalCoupler.ViewModel.GridY);
            var mainSlider = component.GetAllSliders().First();
            var nonLinearConnections = component.WaveLengthToSMatrixMap[RedLaser.WaveLengthInNm].NonLinearConnections;
            var lightIntensity = 1.0;
            MyGameManager.GridViewModel.Grid.IsLightOn = true;
            await MyGameManager.GridViewModel.ShowLightPropagation();
            var lightGloballyOn = await CheckShaderValuesOnRightPinsAsync();
            mainSlider.Value.ShouldBe(0.5);
            lightGloballyOn.OutRightUp.X.ShouldBe((float)(mainSlider.Value * lightIntensity), 0.01);
            lightGloballyOn.OutRightDown.X.ShouldBe((float)((1.0-mainSlider.Value)*lightIntensity),0.01);
            nonLinearConnections.Count.ShouldBe(8); // two for each of the four pins.


        }

        private async Task<(Vector4 OutRightUp, Vector4 OutRightDown)> CheckShaderValuesOnRightPinsAsync()
        {
            await TestScene.GetTree().NextFrame(3);
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            int animationSlotNrUp = 3; // the number of the slot (1-based as the first slot should be on 1)
            int animationSlotNrDown = 4;
            var rightSlots = TunableDirectionalCoupler.OverlayManager.AnimationSlots.Where(slot =>
                      slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm
                      && slot.Side == RectSide.Right
                      && (ShaderMaterial)slot.BaseOverlaySprite?.Material != null
                      ).ToList();
            var rightUpSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(1, 0));
            var rightDownSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(1, 1));
            Vector4 rightUpLight = GetLightAtSlot(rightUpSlot, animationSlotNrUp);
            Vector4 rightDownLight = GetLightAtSlot(rightDownSlot, animationSlotNrDown);
            return (rightUpLight, rightDownLight);
        }

        private static Vector4 GetLightAtSlot(AnimationSlot rightUpSlot, int animationSlotNumber)
        {
            var rightSlotShader = (ShaderMaterial)rightUpSlot.BaseOverlaySprite.Material ?? throw new Exception("Shader is not properly assigned to Overlay");
            return (Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightOutFlow + animationSlotNumber);
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
