using Godot;
using ConnectAPIC.Scripts.Helpers;


#if DEBUG
using System.Reflection;
using Chickensoft.GoDotTest;
#endif

namespace ConnectAPIC;
public partial class Main : Node2D
{
	public TestEnvironment Environment = default!;
	[Export] public PackedScene GameEntryPointScene;

	public override void _Ready()
	{
		GD.Print("started Main Scene");
		this.CheckForNull(x => x.GameEntryPointScene);

#if DEBUG
		// If this is a debug build, use GoDotTest to examine the
		// command line arguments and determine if we should run tests.
		Environment = TestEnvironment.From(OS.GetCmdlineArgs());
		if (Environment.ShouldRunTests)
		{
			CallDeferred("RunTests");
			return;
		}
#endif
		// If we don't need to run tests, we can just switch to the game scene.
		GetTree().ChangeSceneToPacked(GameEntryPointScene);
	}

	private void RunTests()
	  => _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);
}
