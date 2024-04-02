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

namespace ConnectAPIC.test.src
{
    public class DragDropGroupTests : TestClass
    {
        private readonly ILog _log = new GDLog(nameof(GratingCouplerTests));
        public Fixture MyFixture { get; set; }
        public GameManager MyGameManager { get; set; }
        public DragDropGroupTests(Node testScene) : base(testScene) { }
        public ComponentView GratingCoupler { get; set; }
        public ComponentView GratingCoupler1 { get; private set; }

        [Setup]
        public async Task Setup()
        {
            MyFixture = new Fixture(TestScene.GetTree());
            MyGameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor/PICEditor.tscn");
            // find proper tool from component factory
            int gratingComponentNr = MyGameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.Identifier == "GratingCoupler").Key;
            // instantiate tool at the height of the laserInput
            var firstLaserInput = MyGameManager.Grid.ExternalPortManager.ExternalPorts[0];
            var firstInputTileY = firstLaserInput.TilePositionY;

            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(gratingComponentNr, 0, firstInputTileY, DiscreteRotation.R0));
            await MyGameManager.GridViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(gratingComponentNr, 1, firstInputTileY, DiscreteRotation.R0));
            GratingCoupler = MyGameManager.GridView.GridComponentViews[0, firstInputTileY];
            GratingCoupler1 = MyGameManager.GridView.GridComponentViews[0, firstInputTileY];

        }
    }
}
