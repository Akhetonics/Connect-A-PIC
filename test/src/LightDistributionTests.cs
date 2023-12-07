using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using Godot;
using GodotTestDriver;
using System;
using System.Threading.Tasks;

public class LightDistributionTests : TestClass
{
	private readonly ILog _log = new GDLog(nameof(ExampleTest));
    public Fixture MyFixture { get; set; }
    public GameManager gameManager { get; set; }
	public LightDistributionTests(Node testScene) : base(testScene) { }

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
		gameManager._Ready();
		// create a curve at the position of one of the standardInputs and rotate it by 90 degrees and then start light distribution



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
