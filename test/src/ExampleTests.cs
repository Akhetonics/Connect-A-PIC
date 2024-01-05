using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Godot;

namespace ConnectAPIC.test.src
{
	public class ExampleTest : TestClass
	{
		private readonly ILog _log = new GDLog(nameof(ExampleTest));

		public ExampleTest(Node testScene) : base(testScene) { }

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
		  _log.Print("Runs whenever any of the tests in this suite fail.");
	}
}