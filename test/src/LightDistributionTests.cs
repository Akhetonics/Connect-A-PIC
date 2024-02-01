using CAP_Core.Components;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using GodotTestDriver;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Linq;
using ConnectAPIC.LayoutWindow.View;
using System.Collections.Generic;
using ConnectAPIC.Scripts.Helpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;

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
                MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor.tscn");
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
            var firstLaserInput = MyGameManager.Grid.ExternalPorts[0];
            var secondLaserInput = MyGameManager.Grid.ExternalPorts[1];
            var firstInputTileY = firstLaserInput.TilePositionY;
            var secondInputTileY = secondLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;
            GreenLaser = (secondLaserInput as ExternalInput).LaserType;

            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightComponentNr, 0, secondInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightComponentNr, 1, secondInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(curveComponentNr, 0, firstInputTileY, DiscreteRotation.R270));
            // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
            RotatedCurve = MyGameManager.GridViewModel.GridComponentViews[0, firstInputTileY];
            StraightLine = MyGameManager.GridViewModel.GridComponentViews[0, secondInputTileY];
            SecondStraightLine = MyGameManager.GridViewModel.GridComponentViews[1, secondInputTileY];
            var usedPorts = MyGameManager.Grid.GetUsedExternalInputs();

            // Assert if loading has worked properly
            usedPorts.Count.ShouldBe(2);
        }
        [Test]
        public void ComponentRotationTests()
        {
            var outflowSide = CAP_Core.Tiles.RectSide.Up;
            var inflowSide = CAP_Core.Tiles.RectSide.Left;

            var lightAtPins = new List<LightAtPin>() {
                new (0,0,inflowSide,RedLaser,1,0),
                new (0, 0, outflowSide, RedLaser, 0, 1),
            };
            RotatedCurve.DisplayLightVector(lightAtPins);
            MyGameManager.GridViewModel.ShowLightPropagationAsync();
            RotatedCurve.AnimationSlots[0].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.AnimationSlots[1].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.AnimationSlots[2].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
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
            var compModel = MyGameManager.Grid.GetComponentAt(SecondStraightLine.GridX, SecondStraightLine.GridY);
            var innerConnections = compModel.WaveLengthToSMatrixMap[GreenLaser.WaveLengthInNm].GetNonNullValues();
            // then test the light distribution
            SecondStraightLine.HideLightVector();
            SecondStraightLine.DisplayLightVector(lightAtPins);
            var lightLocallyOn = GetInOutLightValueLeft();
            await MyGameManager.GridViewModel.HideLightPropagation();
            var lightGloballyOff = GetInOutLightValueLeft();
            await MyGameManager.GridViewModel.ShowLightPropagationAsync();
            var lightGloballyOn= GetInOutLightValueLeft();
            SecondStraightLine.HideLightVector();
            var lightLocallyOff= GetInOutLightValueLeft();

            // Assert
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

        private (Vector4 In, Vector4 Out) GetInOutLightValueLeft()
        {
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            var rightSlotShader = ((ShaderMaterial) SecondStraightLine.AnimationSlots.Single(slot =>
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
            GameManager.instance = null;
            MyFixture.Cleanup();
        }
        [Failure]
        public void Failure() =>
          _log.Print("something might have gone wrong");
    }
}