using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using CAP_Core.Tiles;
using Chickensoft.GodotTestDriver.Util;

namespace ConnectAPIC.test.src
{
    public class DirectionalCouplerPhysicsTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExampleTest));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public ComponentView DirectionalCouplerLightSwitching { get; set; }
        public ComponentView DirectionalCouplerPowerPreserving { get; set; }
        public LaserType RedLaser { get; set; }
        public DirectionalCouplerPhysicsTests(Node testScene) : base(testScene)
        { }
        public void OnResolved()
        {

        }
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

            // first import all components so that we have curves. 
            // find proper tool from component factory
            int directionalCouplerNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "DirectionalCoupler").Key;
            int straightNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "Straight").Key;
            // instantiate tool at position attached to laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;

            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightNr, 0, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 1, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 3, firstInputTileY, DiscreteRotation.R0));

            // add four more to test if power adds up or not
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 5, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 7, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 9, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(directionalCouplerNr, 11, firstInputTileY, DiscreteRotation.R0));
            DirectionalCouplerLightSwitching = MyGameManager.GridView.GridComponentViews[3, firstInputTileY];
            // set all slides to 0.75 for testing power loss
            for( int x = 5; x < 12; x++)
            {
                var component = MyGameManager.Grid.GetComponentAt(x, firstInputTileY);
                component.GetSlider(0).Value = 0.75;
            }
            DirectionalCouplerPowerPreserving = MyGameManager.GridView.GridComponentViews[11, firstInputTileY];

            var usedPorts = MyGameManager.Grid.GetUsedExternalInputs();
        }

        [Test]
        public async Task TestLightSwitching()
        {
            var lightOnIntensity  = 1;
            // act
            // first test if the connectionWeights are proper
            var compModel = MyGameManager.Grid.GetComponentAt(DirectionalCouplerLightSwitching.ViewModel.GridX, DirectionalCouplerLightSwitching.ViewModel.GridY);
            var innerConnections = compModel.WaveLengthToSMatrixMap[RedLaser.WaveLengthInNm].GetNonNullValues();
            // then test the light distribution
            await MyGameManager.GridView.ShowLightPropagation();
            var lightGloballyOnUp = await GetLightOnRightDownSide(DirectionalCouplerLightSwitching , new Vector2I(1,0) , 3);
            var lightGloballyOnDown = await GetLightOnRightDownSide(DirectionalCouplerLightSwitching, new Vector2I(1,1) , 4);

            lightGloballyOnUp.In.X.ShouldBe(0f, 0.01);
            lightGloballyOnUp.Out.X.ShouldBe(0f, 0.01);
            lightGloballyOnDown.In.X.ShouldBe(0f, 0.01);
            lightGloballyOnDown.Out.X.ShouldBe(lightOnIntensity, 0.01);
        }

        [Test]
        public async Task TestPowerPreserving()
        {
            var compModel = MyGameManager.Grid.GetComponentAt(DirectionalCouplerPowerPreserving.ViewModel.GridX, DirectionalCouplerLightSwitching.ViewModel.GridY);
            var innerConnections = compModel.WaveLengthToSMatrixMap[RedLaser.WaveLengthInNm].GetNonNullValues();
            // then test the light distribution
            await MyGameManager.GridView.ShowLightPropagation();
            var lightGloballyOnUp = await GetLightOnRightDownSide(DirectionalCouplerPowerPreserving, new Vector2I(1, 0), 3);
            var lightGloballyOnDown = await GetLightOnRightDownSide(DirectionalCouplerPowerPreserving, new Vector2I(1, 1), 4);

            lightGloballyOnUp.In.X.ShouldBe(0);
            ((double)(lightGloballyOnUp.Out.X + lightGloballyOnDown.Out.X)).ShouldBeLessThan(1.00001);
            lightGloballyOnDown.In.X.ShouldBe(0);
        }


        private async Task<(Vector4 In, Vector4 Out)> GetLightOnRightDownSide(ComponentView component , Vector2I offset , int overlayNumber)
        {
            await TestScene.GetTree().NextFrame(2);
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            var rightSlotShader = ((ShaderMaterial)component.OverlayManager.AnimationSlots.Single(slot =>
                    slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm
                    && slot.TileOffset == offset
                    && (ShaderMaterial)slot.BaseOverlaySprite?.Material != null
                    && slot.Side == RectSide.Right)
                .BaseOverlaySprite.Material) ?? throw new Exception("Shader is not properly assigned to Overlay");
            var rightIn = (Godot.Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightInFlow + overlayNumber);
            var rightOut = (Godot.Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightOutFlow + overlayNumber);
            return (rightIn, rightOut);
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
