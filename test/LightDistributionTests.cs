using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using ConnectAPic.LayoutWindow;
using Godot;
using GodotTestDriver;
using System.Threading.Tasks;

public class LightDistributionTests : TestClass
{
	private readonly ILog _log = new GDLog(nameof(ExampleTest));
    public Fixture MyFixture { get; set; }
    public PackedScene MainScene { get; set; }
	public LightDistributionTests(Node testScene) : base(testScene) { }

	[SetupAll]
	public async Task SetupAll()
	{
		MyFixture = new Fixture(TestScene.GetTree());
		//MainScene = await MyFixture.LoadAndAddScene<PackedScene>();

        // first import all components so that we have curves. 
        GameManager.Instance._Ready();
		// create a curve at the position of one of the standardinputs and rotate it by 90 degrees and then start light distribution
		

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
