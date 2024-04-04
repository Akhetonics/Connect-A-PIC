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
            await TestScene.GetTree().NextFrame(2);
        }
        [Test]
        public async Task TestLightVectorAssignment()
        {
            // select the two elements in the grid
            await TestScene.GetTree().NextFrame();
            var screenPosLeft = MyGameManager.GridView.ToGlobal(MyGameManager.GridView.MapToLocal(ComponentLeftPos));
            var screenPosRight = MyGameManager.GridView.ToGlobal(MyGameManager.GridView.MapToLocal(ComponentRightPos));
            MyGameManager.GetViewport().DragMouse(screenPosRight - new Vector2(-40, -40), screenPosLeft, MouseButton.Left);
            await TestScene.GetTree().NextFrame();
            // drag the items two tiles to the right
            var moveTilesVector = new Vector2I(3, 0);
            var dragTarget = screenPosLeft + moveTilesVector * GameManager.TilePixelSize;
            MyGameManager.GetViewport().MoveMouseTo(screenPosLeft);
            var data = MyGameManager.GridView.DragDropProxy._GetDragData(MyGameManager.GridView.GetLocalMousePosition());
            await TestScene.GetTree().NextFrame();
            MyGameManager.GetViewport().MoveMouseTo(dragTarget);
            await TestScene.GetTree().NextFrame();
            MyGameManager.GridView.DragDropProxy._DropData(MyGameManager.GridView.GetLocalMousePosition(), data);
            await TestScene.GetTree().NextFrame();
            // check if they have both moved (deleted and newly created)
            var oldLeft = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentRightPos.X, ComponentRightPos.Y);
            var oldRight = MyGameManager.Grid.ComponentMover.GetComponentAt(ComponentRightPos.X, ComponentRightPos.Y);
            var newPosLeft = ComponentLeftPos += moveTilesVector;
            var newPosRight = ComponentRightPos += moveTilesVector;
            var foundLeft = MyGameManager.Grid.ComponentMover.GetComponentAt(newPosLeft.X, newPosLeft.Y);
            var foundRight = MyGameManager.Grid.ComponentMover.GetComponentAt(newPosRight.X, newPosRight.Y);
            await TestScene.GetTree().NextFrame(100);
            // assert

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
