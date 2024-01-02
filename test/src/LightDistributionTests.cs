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

public class LightDistributionTests : TestClass
{
	private readonly ILog _log = new GDLog(nameof(ExampleTest));
    public Fixture MyFixture { get; set; }
    public GameManager gameManager { get; set; }
	public LightDistributionTests(Node testScene) : base(testScene) { }
    public ComponentView TestComponent { get; set; }
    public void OnResolved()
    {

    }
	[SetupAll]
	public async Task SetupAll()
	{
		MyFixture = new Fixture(TestScene.GetTree());
		try
		{
            gameManager = await MyFixture.LoadAndAddScene<GameManager>("res://Scenes/PICEditor.tscn");
        } catch (Exception ex)
		{
			_log.Print(ex.Message);
        }

        // first import all components so that we have curves. 
        // find proper tool from toolbox
        int componentNr = gameManager.GridView.ComponentViewFactory.PackedComponentCache.Single(c=>c.Value.Draft.identifier == "Bend").Key;
        // instantiate tool at position attached to laserInput
        var firstLaserInput = gameManager.Grid.ExternalPorts.First();
        var TileY= firstLaserInput.TilePositionY;
        
        gameManager.GridViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(componentNr, 0, TileY, DiscreteRotation.R270));
        // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
        TestComponent = gameManager.GridViewModel.GridComponentViews[0, TileY];
        var usedPorts = gameManager.Grid.GetUsedExternalInputs();

        // Assert
        TestComponent.AnimationSlots.First().Rotation.ShouldBe(TestComponent.RotationCC);
        usedPorts.Count.ShouldBe(1);
    }
    [Setup]
    public void Setup() => _log.Print("Setup");
    [Test]
    public void Test()
    {
        var outflowSide = CAP_Core.Tiles.RectSide.Up;
        var inflowSide = CAP_Core.Tiles.RectSide.Left;
        var laserType = new CAP_Core.ExternalPorts.LaserType(CAP_Core.ExternalPorts.LightColor.Red, 43);
        var lightVector = new List<LightAtPin>() {
            new (0,0,inflowSide,laserType,1,0),
            new (0,0,outflowSide,laserType,0,1),
        };
        var expectedLaserOutputColor = laserType.Color.ToGodotColor();
        TestComponent.DisplayLightVector(lightVector);
        gameManager.GridViewModel.ShowLightPropagation();
        var shaderLightValue = TestComponent.GetShaderLightVector(TestComponent.AnimationSlots.FirstOrDefault());
        
        
        shaderLightValue.ShouldBe(expectedLaserOutputColor, "because the shadervalue of the view was set by the model");
    }
    [Cleanup]
    public void Cleanup() => _log.Print("Cleanup");
    [CleanupAll]
    public void CleanupAll() => _log.Print("Cleanup everything");
    [Failure]
    public void Failure() =>
	  _log.Print("something might have gone wrong");
}
