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

namespace ConnectAPIC.test.src
{
    public class LightDistributionTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExampleTest));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public LightDistributionTests(Node testScene) : base(testScene) { }
        public ComponentView RotatedCurve { get; set; }
        public ComponentView StraightLine { get; set; }
        public LaserType RedLaser { get; set; }
        public LaserType GreenLaser { get; set; }
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
            int curveComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.identifier == "Bend").Key;
            int straightComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.identifier == "Straight").Key;
            // instantiate tool at position attached to laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPorts[0];
            var secondLaserInput = MyGameManager.Grid.ExternalPorts[1];
            var firstInputTileY = firstLaserInput.TilePositionY;
            var secondInputTileY = secondLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;
            GreenLaser = (secondLaserInput as ExternalInput).LaserType;

            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightComponentNr, 0, secondInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(curveComponentNr, 0, firstInputTileY, DiscreteRotation.R270));
            // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
            RotatedCurve = MyGameManager.GridViewModel.GridComponentViews[0, firstInputTileY];
            StraightLine = MyGameManager.GridViewModel.GridComponentViews[0, secondInputTileY];
            var usedPorts = MyGameManager.Grid.GetUsedExternalInputs();

            // Assert if loading has worked properly
            usedPorts.Count.ShouldBe(2);
        }
        [Test]
        public void TestLightVectorAssignment()
        {
            var outflowSide = CAP_Core.Tiles.RectSide.Up;
            var inflowSide = CAP_Core.Tiles.RectSide.Left;

            var upperLightVector = new LightAtPin(0, 0, outflowSide, RedLaser, 0, 1);
            var lightAtPins = new List<LightAtPin>() {
            new (0,0,inflowSide,RedLaser,1,0),
            upperLightVector,
        };
            RotatedCurve.DisplayLightVector(lightAtPins);
            MyGameManager.GridViewModel.ShowLightPropagation();
            RotatedCurve.AnimationSlots[0].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.AnimationSlots[1].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
            RotatedCurve.AnimationSlots[2].Rotation.ShouldBe(RotatedCurve.RotationCC, "AnimationSlot should rotate according to the rotation of the component");
        }
        [Test]
        public void TestShaderAssignment()
        {
            var outflowSide = CAP_Core.Tiles.RectSide.Right;
            var inflowSide = CAP_Core.Tiles.RectSide.Left;
            var rightLightVector = new LightAtPin(0, 0, outflowSide, GreenLaser, 0, 1);
            var lightAtPins = new List<LightAtPin>() {
            new (0,0,inflowSide,GreenLaser,1,0),
            rightLightVector,
        };
            StraightLine.HideLightVector();
            StraightLine.DisplayLightVector(lightAtPins);
            TestStraightShaderParameters(GreenLaser, 1);
            MyGameManager.GridViewModel.HideLightPropagation();
            MyGameManager.GridViewModel.ShowLightPropagation();
            TestStraightShaderParameters(GreenLaser, 1);
            StraightLine.HideLightVector();
            TestStraightShaderParameters(GreenLaser, 0);
        }
        private void TestStraightShaderParameters(LaserType activatedLaser, float expectedLightMagnitude)
        {
            foreach (var slot in StraightLine.AnimationSlots)
            {
                if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
                {
                    var inflowAndPosition = (Godot.Vector4)shaderMat.GetShaderParameter(ShaderParameterNames.LightInFlow + 1);
                    var outflowAndPosition = (Godot.Vector4)shaderMat.GetShaderParameter(ShaderParameterNames.LightOutFlow + 1);
                    if (slot.MatchingLaser.WaveLengthInNm == activatedLaser.WaveLengthInNm)
                    {
                        if (slot.Side == CAP_Core.Tiles.RectSide.Left)
                        {
                            inflowAndPosition.X.ShouldBe(expectedLightMagnitude, $"Inflow should be {expectedLightMagnitude} on the left side (inflow side) for laser: " + slot.MatchingLaser);
                            outflowAndPosition.X.ShouldBe(0, "Outflow should be 0 on the left side");
                        }
                        if (slot.Side == CAP_Core.Tiles.RectSide.Right)
                        {
                            inflowAndPosition.X.ShouldBe(0, "Inflow should be 0 on the right side");
                            outflowAndPosition.X.ShouldBe(expectedLightMagnitude, $"outflow should be {expectedLightMagnitude} on the right side (inflow side) for laser: " + slot.MatchingLaser);
                        }
                    }
                    else
                    {
                        inflowAndPosition.X.ShouldBe(0, "inflow must be zero for Laser: " + slot.MatchingLaser);
                        outflowAndPosition.X.ShouldBe(0, "inflow must be zero for Laser: " + slot.MatchingLaser);
                    }

                }
            }
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