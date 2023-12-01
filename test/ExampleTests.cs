using CAP_Contracts.Logger;
using CAP_Core;
using Chickensoft.GoDotLog;
using Chickensoft.GoDotTest;
using Godot;
using System.Diagnostics;

public class ExampleTest : TestClass
{
	private readonly ILogger _log = new Logger(log=>Debug.WriteLine(log.ToString()));

	public ExampleTest(Node testScene) : base(testScene) { }

	[SetupAll]
	public void SetupAll() => _log.Print("Setup everything");

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
