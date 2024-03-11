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
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using CAP_Core.Components.ComponentHelpers;

namespace ConnectAPIC.test.src
{
    public class MMI3x3Tests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(ExampleTest));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public ComponentView MMI3x3 { get; set; }
        public LaserType RedLaser { get; set; }
        public MMI3x3Tests(Node testScene) : base(testScene) { }
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
            int MMI3x3NR = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "MMI3x3").Key;

            // instantiate tool at position attached to laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;
            RedLaser = (firstLaserInput as ExternalInput).LaserType;
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(MMI3x3NR, 0, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(MMI3x3NR, 3, firstInputTileY, DiscreteRotation.R0));
            MMI3x3 = MyGameManager.GridViewModel.GridComponentViews[3, firstInputTileY];
        }

        [Test]
        public async Task TestMMI3x3LightFields()
        {
            var component = MyGameManager.Grid.GetComponentAt(MMI3x3.ViewModel.GridX, MMI3x3.ViewModel.GridY);
            var lightPower = (float)MyGameManager.Grid.GetUsedExternalInputs().FirstOrDefault(i => i.Input.LaserType == LaserType.Red).Input.InFlowPower.Magnitude;
            MyGameManager.GridViewModel.LightCalculator.LightCalculationChanged += (object sender, CAP_Core.LightCalculation.LightCalculationChangeEventArgs e) =>
            {
                var PinUpRight = (Guid)component.PinIdRightOut(2, 0);
                var PinMiddleRight = (Guid)component.PinIdRightOut(2, 1);
                var PinDownRight = (Guid)component.PinIdRightOut(2, 2);
                var FieldUpRight = e.LightFieldVector[PinUpRight];
                var FieldMiddleRight = e.LightFieldVector[PinMiddleRight];
                var FieldDownRight = e.LightFieldVector[PinDownRight];

                FieldUpRight.Magnitude.ShouldBe( Math.Sqrt(lightPower / 3) , 0.00001);
                FieldMiddleRight.Magnitude.ShouldBe(Math.Sqrt(lightPower / 3), 0.00001);
                FieldDownRight.Magnitude.ShouldBe(Math.Sqrt(lightPower / 3), 0.00001);
            };

            MyGameManager.GridViewModel.Grid.IsLightOn = true;
            await MyGameManager.GridViewModel.ShowLightPropagation();
            
        }

        [Test]
        public async Task TestMMI3x3ShaderValues()
        {
            var component = MyGameManager.Grid.GetComponentAt(MMI3x3.ViewModel.GridX, MMI3x3.ViewModel.GridY);
            var lightPower = (float) MyGameManager.Grid.GetUsedExternalInputs().FirstOrDefault(i => i.Input.LaserType == LaserType.Red).Input.InFlowPower.Magnitude;
            MyGameManager.GridViewModel.Grid.IsLightOn = true;
            await MyGameManager.GridViewModel.ShowLightPropagation();
            await TestScene.GetTree().NextFrame(2); // wait some frames to let the shader be applied
            var redSlots = MMI3x3.OverlayManager.AnimationSlots.Where(slot => slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm).ToList();
            var rightUpSlot = redSlots.Single(s => s.TileOffset == new Vector2I(2, 0));
            var rightMiddleSlot = redSlots.Single(s => s.TileOffset == new Vector2I(2, 1));
            var rightDownSlot = redSlots.Single(s => s.TileOffset == new Vector2I(2, 2));
            var leftUpSlot = redSlots.First(s => s.TileOffset == new Vector2I(0, 0));
            var leftMiddleSlot = redSlots.First(s => s.TileOffset == new Vector2I(0, 1));
            var leftDownSlot = redSlots.First(s => s.TileOffset == new Vector2I(0, 2));
            // measure if the animation has the proper alpha values in the shader
            var lightUpLeft = GetLightAtSlot(leftUpSlot, 1);
            var lightMiddleLeft = GetLightAtSlot(leftUpSlot, 2);
            var lightDownLeft = GetLightAtSlot(leftUpSlot, 3);
            var lightUpRight = GetLightAtSlot(rightUpSlot, 7);
            var lightMiddleRight = GetLightAtSlot(rightUpSlot, 8);
            var lightDownRight = GetLightAtSlot(rightUpSlot, 9);

            // assert
            MMI3x3.WidthInTiles.ShouldBe(3);
            MMI3x3.HeightInTiles.ShouldBe(3);
            rightUpSlot.FlowDirection.ShouldBe(FlowDirection.Both);
            rightMiddleSlot.FlowDirection.ShouldBe(FlowDirection.Both);
            rightDownSlot.FlowDirection.ShouldBe(FlowDirection.Both);

            lightUpLeft.inflow.ShouldBe(lightPower/3);
            lightMiddleLeft.inflow.ShouldBe(lightPower / 3);
            lightDownLeft.inflow.ShouldBe(lightPower / 3);
            lightUpRight.inflow.ShouldBe(0);
            lightMiddleRight.inflow.ShouldBe(0);
            lightDownRight.inflow.ShouldBe(0);

            // we can only measure the pins that control the alpha values of the shader - the others will have a value of 0
            lightUpLeft.outflow.ShouldBe(0);
            lightMiddleLeft.outflow.ShouldBe(0);
            lightDownLeft.outflow.ShouldBe(0);
        }

        private static (float inflow, float outflow ) GetLightAtSlot(AnimationSlot rightUpSlot, int animationSlotNumber)
        {
            var rightSlotShader = (ShaderMaterial)rightUpSlot.BaseOverlaySprite.Material ?? throw new Exception("Shader is not properly assigned to Overlay");
            var shaderValueInFlow = (Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightInFlow + animationSlotNumber);
            var shaderValueOutFlow = (Vector4)rightSlotShader.GetShaderParameter(ShaderParameterNames.LightOutFlow + animationSlotNumber);
            return (shaderValueInFlow.X, shaderValueOutFlow.X);
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
