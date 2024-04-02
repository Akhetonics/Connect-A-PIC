using CAP_Core.Components;
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
using CAP_Core.ExternalPorts;
using Chickensoft.GodotTestDriver.Util;
using Shouldly;
using Chickensoft.GodotTestDriver.Input;
using CAP_Core.Helpers;

namespace ConnectAPIC.test.src
{
    public class DragDropGroupTests : TestClass
    {
        private const string StraightCompIdentifier = "Straight";
        private readonly ILog _log = new GDLog(nameof(GratingCouplerTests));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public DragDropGroupTests(Node testScene) : base(testScene) { }
        public ComponentView StraightLeft { get; set; }
        public ComponentView StraightRight { get; private set; }
        public Vector2I ComponentLeftPos { get; private set; }
        public Vector2I ComponentRightPos { get; private set; }

        [Setup]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());
            MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
            // find proper tool from component factory
            int straightComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == StraightCompIdentifier).Key;
            // instantiate tool at the height of the laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPortManager.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;
            ComponentLeftPos = new Vector2I(0,firstInputTileY);
            ComponentRightPos = new Vector2I(1,firstInputTileY);
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightComponentNr, ComponentLeftPos.X, ComponentLeftPos.Y, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(straightComponentNr, ComponentRightPos.X, ComponentRightPos.Y, DiscreteRotation.R0));
            StraightLeft = MyGameManager.GridView.GridComponentViews[ComponentLeftPos.X, ComponentLeftPos.Y];
            StraightRight = MyGameManager.GridView.GridComponentViews[ComponentRightPos.X, ComponentRightPos.Y];
        }
        [Test]
        public async Task TestLightVectorAssignment()
        {
            // select the two elements in the grid
            var screenPosLeft = MyGameManager.GridView.ToGlobal(MyGameManager.GridView.MapToLocal(ComponentLeftPos));
            var screenPosRight = MyGameManager.GridView.ToGlobal(MyGameManager.GridView.MapToLocal(ComponentRightPos));
            MyGameManager.GetViewport().MoveMouseTo(screenPosRight + new Vector2(-40, -40)); // move it a bit out of the object so it can open a selection box properly
            MyGameManager.GetViewport().PressMouse(MouseButton.Left);
            await TestScene.GetTree().NextFrame();
            MyGameManager.GetViewport().MoveMouseTo(screenPosLeft);
            MyGameManager.GetViewport().ReleaseMouse(MouseButton.Left);
            await TestScene.GetTree().NextFrame();

            // drag the items two tiles to the right
            MyGameManager.GetViewport().MoveMouseTo(screenPosRight);
            MyGameManager.GetViewport().PressMouse(MouseButton.Left);
            await TestScene.GetTree().NextFrame();
            MyGameManager.GetViewport().MoveMouseTo(screenPosLeft + new Vector2(GameManager.TilePixelSize * 2,0));
            await TestScene.GetTree().NextFrame();
            MyGameManager.GetViewport().ReleaseMouse(MouseButton.Left);
            await TestScene.GetTree().NextFrame();
            // check if they have both moved (deleted and newly created)
            var oldLeft = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentRightPos.X, ComponentRightPos.Y);
            var oldRight = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentRightPos.X, ComponentRightPos.Y);
            ComponentLeftPos += new Vector2I(2, 0);
            ComponentRightPos += new Vector2I(2, 0);
            var foundLeft = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentLeftPos.X, ComponentLeftPos.Y);
            var foundRight = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentRightPos.X, ComponentRightPos.Y);
            
            oldLeft.ShouldBe(null, "because we moved the left component");
            oldRight.ShouldBe(null, "because we moved the right component");
            foundLeft.Identifier.ShouldBe(StraightCompIdentifier, "because we moved the straight thing there");
            foundRight.Identifier.ShouldBe(StraightCompIdentifier, "because we moved the straight thing there");
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
