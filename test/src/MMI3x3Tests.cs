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
        public async Task TestShaderAssignment()
        {
            var component = MyGameManager.Grid.GetComponentAt(MMI3x3.ViewModel.GridX, MMI3x3.ViewModel.GridY);
            var lightIntensity = 1.0;
            MyGameManager.GridViewModel.Grid.IsLightOn = true;
            await MyGameManager.GridViewModel.ShowLightPropagation();
            var lightGloballyOn = await CheckShaderValuesOnRightPinsAsync();
            var rightSlots = MMI3x3.OverlayManager.AnimationSlots.Where(slot =>
                      slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm
                      && slot.Side == RectSide.Right
                      && (ShaderMaterial)slot.BaseOverlaySprite?.Material != null
                      ).ToList();
            var rightUpSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 0));
            var rightMiddleSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 1));
            var rightDownSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 2));

            // assert
            
            MMI3x3.WidthInTiles.ShouldBe(3);
            MMI3x3.HeightInTiles.ShouldBe(3);
            rightUpSlot.FlowDirection.ShouldBe(FlowDirection.In);
            rightMiddleSlot.FlowDirection.ShouldBe(FlowDirection.In);
            rightDownSlot.FlowDirection.ShouldBe(FlowDirection.In);

            lightGloballyOn.OutRightUp.X.ShouldBe((float)(1 / 3 * lightIntensity));
            lightGloballyOn.OutRightMiddle.X.ShouldBe((float)(1 / 3 * lightIntensity));
            lightGloballyOn.OutRightDown.X.ShouldBe((float)(1 / 3 * lightIntensity), 0.01);
        }

        private async Task<(Vector4 OutRightUp, Vector4 OutRightMiddle, Vector4 OutRightDown)> CheckShaderValuesOnRightPinsAsync()
        {
            await TestScene.GetTree().NextFrame(3);
            // get the shader-light intensity value on the left side
            // because only left is defined in the Straight Component (it only has one set of RGB-Overlays and only uses the left in/out values)
            int animationSlotNrUp = 4; // the number of the slot (1-based as the first slot should be on 1)
            int animationSlotNrMiddle = 5;
            int animationSlotNrDown = 6;
            var rightSlots = MMI3x3.OverlayManager.AnimationSlots.Where(slot =>
                      slot.MatchingLaser.WaveLengthInNm == RedLaser.WaveLengthInNm
                      && slot.Side == RectSide.Right
                      && (ShaderMaterial)slot.BaseOverlaySprite?.Material != null
                      ).ToList();
            var rightUpSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 0));
            var rightMiddleSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 1));
            var rightDownSlot = rightSlots.Single(s => s.TileOffset == new Vector2I(2, 2));
            Vector4 rightUpLight = GetLightAtSlot(rightUpSlot, animationSlotNrUp);
            Vector4 rightMiddleLight = GetLightAtSlot(rightMiddleSlot, animationSlotNrMiddle);
            Vector4 rightDownLight = GetLightAtSlot(rightMiddleSlot, animationSlotNrDown);
            return (rightUpLight, rightMiddleLight, rightDownLight);
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
