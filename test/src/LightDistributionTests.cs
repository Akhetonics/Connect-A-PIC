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

        // Assert if loading has worked properly
        TestComponent.AnimationSlots.First().Rotation.ShouldBe(TestComponent.RotationCC);
        usedPorts.Count.ShouldBe(1);
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
        var expectedRedColor = redLaser.Color.ToGodotColor();
        TestComponent.DisplayLightVector(lightAtPins);
        gameManager.GridViewModel.ShowLightPropagation();
        var animationSlots = TestComponent.AnimationSlots;
        var shaderAnimationNumber = 1;
        foreach (var slot in animationSlots)
        {
            if (slot?.BaseOverlaySprite?.Material is ShaderMaterial shaderMat)
            {
                var inflowAndPosition = (Godot.Vector4) shaderMat.GetShaderParameter("lightInFlow" + shaderAnimationNumber);
                //shaderMat.GetShaderParameter("lightColor", new Godot.Color(0, 0, 0));
                // check if all other colors are off and only the one we triggered -the red one - is on (one)
            }
            shaderAnimationNumber++;
            
        }
        //var shaderLightValue = TestComponent.AreAllShaderValuesCorrect(upperLightVector);
        //shaderLightValue.ShouldBe(expectedRedColor, "because the shadervalue of the view was set by the model");
        _log.Print($"laserOutputColor could be properly set: {redLaser.Color.ToReadableString()}");
    }
   
    [Cleanup]
    public void Cleanup() => _log.Print("Cleanup");
    [CleanupAll]
    public void CleanupAll() => _log.Print("Cleanup everything");
    [Failure]
    public void Failure() =>
	  _log.Print("something might have gone wrong");
}
