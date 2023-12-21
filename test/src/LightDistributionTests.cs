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

public class LightDistributionTests : TestClass
{
	private readonly ILog _log = new GDLog(nameof(ExampleTest));
    public Fixture MyFixture { get; set; }
    public GameManager gameManager { get; set; }
	public LightDistributionTests(Node testScene) : base(testScene) { }

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
        gameManager.GridViewModel.ShowLightPropagation();
        var componentView = gameManager.GridViewModel.GridComponentViews[0, TileY];
        var usedPorts = gameManager.Grid.GetUsedExternalInputs();
        
        // Assert
        componentView.AnimationSlots.First().Rotation.ShouldBe(componentView.RotationCC);
        usedPorts.Count.ShouldBe(1);
    }
    [Setup]
    public void Setup() => _log.Print("Setup");
    [Test]
    public void Test() => _log.Print("Test");
    [Cleanup]
    public void Cleanup() => _log.Print("Cleanup");
    [CleanupAll]
    public void CleanupAll() => _log.Print("Cleanup everything");
    [Failure]
    public void Failure() =>
	  _log.Print("something might have gone wrong");
}
