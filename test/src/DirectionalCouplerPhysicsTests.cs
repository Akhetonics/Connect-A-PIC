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
        public ComponentView LastDirectionalCoupler { get; set; }
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
            LastDirectionalCoupler = MyGameManager.GridViewModel.GridComponentViews[3, firstInputTileY];

            var usedPorts = MyGameManager.Grid.GetUsedExternalInputs();
        }

        [Test]
        public async Task TestShaderAssignment()
        {
            var lightOnIntensity  = 1;
            // act
            // first test if the connectionWeights are proper
            var compModel = MyGameManager.Grid.GetComponentAt(LastDirectionalCoupler.ViewModel.GridX, LastDirectionalCoupler.ViewModel.GridY);
            var innerConnections = compModel.WaveLengthToSMatrixMap[RedLaser.WaveLengthInNm].GetNonNullValues();
            // then test the light distribution
            await MyGameManager.GridViewModel.ShowLightPropagation();
            var lightGloballyOnUp = await GetLightOnRightDownSide(new Vector2I(1,0));
            var lightGloballyOnDown = await GetLightOnRightDownSide(new Vector2I(1,1));

            lightGloballyOnUp.In.X.ShouldBe(0);
            lightGloballyOnUp.Out.X.ShouldBe(0);
            lightGloballyOnDown.In.X.ShouldBe(0);
            lightGloballyOnDown.Out.X.ShouldBe(lightOnIntensity, 0.01);
        }

        private async Task<(Vector4 In, Vector4 Out)> GetLightOnRightDownSide(Vector2I offset)
        {
            await TestScene.GetTree().NextFrame(2);
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            var rightSlotShader = ((ShaderMaterial)LastDirectionalCoupler.AnimationSlots.Single(slot =>
                    slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm
                    && slot.TileOffset == offset
                    && (ShaderMaterial)slot.BaseOverlaySprite?.Material != null
                    && slot.Side == RectSide.Right)
                .BaseOverlaySprite.Material) ?? throw new Exception("Shader is not properly assigned to Overlay");
            var rightIn = (Godot.Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightInFlow + 1);
            var rightOut = (Godot.Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightOutFlow + 1);
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
