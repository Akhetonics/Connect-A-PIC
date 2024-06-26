using CAP_Core.Components;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Linq;
using ConnectAPIC.LayoutWindow.View;
using System.Collections.Generic;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using Chickensoft.GodotTestDriver;
using Chickensoft.GodotTestDriver.Util;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;

namespace ConnectAPIC.test.src
{
    public class LightDistributionTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExampleTest));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public ComponentView RotatedCurve { get; set; }
        public ComponentView StraightLine { get; set; }
        public ComponentView SecondStraightLine { get; set; }
        public LaserType RedLaser { get; set; }
        public LaserType GreenLaser { get; set; }
        public LightDistributionTests(Node testScene) : base(testScene) 
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
            int curveComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "Bend").Key;
            int straightComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "Straight").Key;
            // instantiate tool at position attached to laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPortManager.ExternalPorts[0];
            var secondLaserInput = MyGameManager.Grid.ExternalPortManager.ExternalPorts[1];
            var firstInputTileY = firstLaserInput.TilePositionY;
            var secondInputTileY = secondLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;
            GreenLaser = (secondLaserInput as ExternalInput).LaserType;

            await MyGameManager.GridViewModel.CommandFactory.CreateCommand(CommandType.CreateComponent).ExecuteAsync(new CreateComponentArgs(straightComponentNr, 0, secondInputTileY, DiscreteRotation.R0, Guid.NewGuid()));
            await MyGameManager.GridViewModel.CommandFactory.CreateCommand(CommandType.CreateComponent).ExecuteAsync(new CreateComponentArgs(straightComponentNr, 1, secondInputTileY, DiscreteRotation.R0, Guid.NewGuid()));
            await MyGameManager.GridViewModel.CommandFactory.CreateCommand(CommandType.CreateComponent).ExecuteAsync(new CreateComponentArgs(curveComponentNr, 0, firstInputTileY, DiscreteRotation.R270, Guid.NewGuid()));
            // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
            RotatedCurve = MyGameManager.GridView.GridComponentViews[0, firstInputTileY];
            StraightLine = MyGameManager.GridView.GridComponentViews[0, secondInputTileY];
            SecondStraightLine = MyGameManager.GridView.GridComponentViews[1, secondInputTileY];
            var usedPorts = MyGameManager.Grid.ExternalPortManager.GetUsedExternalInputs();

            // Assert if loading has worked properly
            usedPorts.Count.ShouldBe(2);
        }
        [Test]
        public async Task ComponentRotationTests()
        {
            var outflowSide = CAP_Core.Tiles.RectSide.Up;
            var inflowSide = CAP_Core.Tiles.RectSide.Left;

            var lightAtPins = new List<LightAtPin>() {
                new (0,0,inflowSide,RedLaser,1,0),
                new (0, 0, outflowSide, RedLaser, 0, 1),
            };
            RotatedCurve.ViewModel.DisplayLightVector(lightAtPins);
            await MyGameManager.GridView.ShowLightPropagation() ;
            await TestScene.GetTree().NextFrame(10);
            RotatedCurve.OverlayManager.AnimationSlots[0].Rotation.ShouldBe(RotatedCurve.ViewModel.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.OverlayManager.AnimationSlots[1].Rotation.ShouldBe(RotatedCurve.ViewModel.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.OverlayManager.AnimationSlots[2].Rotation.ShouldBe(RotatedCurve.ViewModel.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
        }
        [Test]
        public async Task TestShaderAssignment()
        {
            // setup
            float lightOnIntensity = 1;
            var rightLightVector = new LightAtPin(0, 0, RectSide.Right, GreenLaser, 0, lightOnIntensity);
            var leftLightVector = new LightAtPin(0, 0, RectSide.Left, GreenLaser, lightOnIntensity, 0);
            var lightAtPins = new List<LightAtPin>() { leftLightVector, rightLightVector, };

            // act
            // first test if the connectionWeights are proper
            var compModel = MyGameManager.Grid.ComponentMover.GetComponentAt(SecondStraightLine.ViewModel.GridX, SecondStraightLine.ViewModel.GridY);
            var innerConnections = compModel.WaveLengthToSMatrixMap[GreenLaser.WaveLengthInNm].GetNonNullValues();
            // then test the light distribution
            SecondStraightLine.HideLightVector();
            SecondStraightLine.ViewModel.DisplayLightVector(lightAtPins);
            var lightLocallyOn = await GetInOutLightValueLeft();
            await MyGameManager.GridView.HideLightPropagation();
            var lightGloballyOff = await GetInOutLightValueLeft();
            await MyGameManager.GridView.ShowLightPropagation();
            var lightGloballyOn= await GetInOutLightValueLeft();
            SecondStraightLine.HideLightVector();
            var lightLocallyOff= await GetInOutLightValueLeft();
            
            innerConnections.First().Value.Magnitude.ShouldBe(1, 0.01);
            lightLocallyOn.In.X.ShouldBe(lightOnIntensity, 0.01);
            lightLocallyOn.Out.X.ShouldBe(0,0.0001, "because not light comes from a right positioned component");
            lightGloballyOn.In.X.ShouldBe(lightOnIntensity, 1);
            lightGloballyOn.Out.X.ShouldBe(0);
            lightLocallyOff.In.X.ShouldBe(0);
            lightLocallyOff.Out.X.ShouldBe(0);
            lightGloballyOff.In.X.ShouldBe(0);
            lightGloballyOff.Out.X.ShouldBe(0);
            
        }

        private async Task<(Vector4 In, Vector4 Out)> GetInOutLightValueLeft()
        {
            await TestScene.GetTree().NextFrame(2);
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            var rightSlotShader = ((ShaderMaterial) SecondStraightLine.OverlayManager.AnimationSlots.Single(slot =>
                      slot.MatchingLaser.WaveLengthInNm == GreenLaser.WaveLengthInNm
                   && (ShaderMaterial) slot.BaseOverlaySprite?.Material != null
                   && slot.Side == RectSide.Left)
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
