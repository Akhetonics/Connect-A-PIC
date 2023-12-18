using CAP_Core.Component.ComponentHelpers;
using CAP_Core;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using GodotTestDriver;
using Shouldly;
using System;
using System.Threading.Tasks;
using System.Linq;
using Chickensoft.AutoInject;
using SuperNodes.Types;
using ConnectAPIC.LayoutWindow.ViewModel;

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
        gameManager._EnterTree();
        gameManager._Ready();
        // find proper tool from toolbox

        // instantiate tool at position attached to laserinput
        var TileY= gameManager.Grid.GetUsedStandardInputs().First().Input.TilePositionY;
        int componentNr = 0;
        
        gameManager.GridViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(componentNr, 0, TileY, DiscreteRotation.R90));
        // create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution
        gameManager.ShouldNotBe(null);
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
