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

public class GratingCouplerTests : TestClass
{
    private readonly ILog _log = new GDLog(nameof(GratingCouplerTests));
    public Fixture MyFixture { get; set; }
    public GameManager GameManager { get; set; }
    public GratingCouplerTests(Node testScene) : base(testScene) { }
    public ComponentView GratingCoupler { get; set; }
    public void OnResolved()
    {

    }
    [SetupAll]
    public async Task SetupAll()
    {
        MyFixture = new Fixture(TestScene.GetTree());
        GameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor.tscn");
        // find proper tool from component factory
        int gratingComponentNr = GameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c => c.Value.Draft.identifier == "GratingCoupler").Key;
        // instantiate tool at the height of the laserInput
        var firstLaserInput = GameManager.Grid.ExternalPorts[0];
        var firstInputTileY = firstLaserInput.TilePositionY;

        GameManager.GridViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(gratingComponentNr, 0, firstInputTileY, DiscreteRotation.R0));
        GratingCoupler = GameManager.GridViewModel.GridComponentViews[0, firstInputTileY];
        
    }
    [Setup]
    public void Setup() => _log.Print("Setup");
    [Test]
    public void TestLightVectorAssignment()
    {
        var outflowSide = CAP_Core.Tiles.RectSide.Up;
        var inflowSide = CAP_Core.Tiles.RectSide.Left;
        var redLaser = LaserType.Red;
        var upperLightVector = new LightAtPin(0, 0, outflowSide, redLaser, 0, 1);
        var lightAtPins = new List<LightAtPin>() {
            new (0,0,inflowSide,redLaser,1,0),
            upperLightVector,
        };
        GratingCoupler.ShouldNotBeNull("the gratingcoupler should have been loaded successfully but it didn't" );
        GratingCoupler.DisplayLightVector(lightAtPins);
        GameManager.GridViewModel.ShowLightPropagation();
        var inflowShaderParam = (Vector4)(GratingCoupler.AnimationSlots[0].BaseOverlaySprite.Material as ShaderMaterial).GetShaderParameter(ShaderParameterNames.LightInFlow + 1);
        inflowShaderParam.X.ShouldBe(1,"because we have set the light input to be 1 for red laser ");
    }

    [Cleanup]
    public void Cleanup() => _log.Print("Cleanup");
    [CleanupAll]
    public void CleanupAll() => _log.Print("Cleanup everything");
    [Failure]
    public void Failure() =>
      _log.Print("something might have gone wrong");
}
