using CAP_Contracts.Logger;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using Chickensoft.AutoInject;
using SuperNodes.Types;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPic.LayoutWindow;

namespace ConnectAPIC.Scenes.InGameConsole
{
    [SuperNode(typeof(Dependent))]
    public partial class GameConsole : ScrollContainer
    {
        public override partial void _Notification(int what);
        [Dependency] public GridViewModel ViewModel => DependOn<GridViewModel>();
        [Export] private Node LoggingParent { get; set; }
        [Export] private RichTextLabel InfoTextTemplate { get; set; }
        [Export] private RichTextLabel ErrorTextTemplate { get; set; }
        public ILogger Logger { get; private set; }

        private bool visibilityChanged = false;

        public override void _Ready()
        {
            this.CheckForNull(x => LoggingParent, Logger);
            this.CheckForNull(x => InfoTextTemplate, Logger);
            this.CheckForNull(x => ErrorTextTemplate, Logger);
            Hide();
        }
        public void Initialize(ILogger logger)
        {
            this.Logger = logger;
            logger.LogAdded += (Log obj) =>
            {
                if (obj.Level > LogLevel.Warn)
                {
                    PrintErr(obj.Message);
                }
                else
                {
                    PrintInfo(obj.Message);
                }
            };
        }

        public void PrintInfo(string text)
        {
            GD.Print(text);
            CallDeferred(nameof(Print),text, false); // make sure it runs on the UI thread
        }
        public void PrintErr(string text)
        {
            CallDeferred(nameof(Print),text, true);// make sure it runs on the UI thread
            GD.PrintErr(text);
        }
        private void Print(string text, bool isError = false)
        {
            RichTextLabel labelTemplate = InfoTextTemplate;
            if (isError)
            {
                labelTemplate = ErrorTextTemplate;
                Show();
            }

            var newLine = (RichTextLabel)labelTemplate.Duplicate();
            newLine.Text = text;
            newLine.Visible = true;
            LoggingParent.AddChild(newLine);
            LoggingParent.MoveChild(newLine, 0);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.Pressed && eventKey.IsCommandOrControlPressed() && eventKey.Keycode == Key.F1)
                {
                    if (visibilityChanged == false)
                    {
                        visibilityChanged = true;
                        Visible = !Visible;
                    }
                }
                else
                {
                    visibilityChanged = false;
                }

				if (eventKey.IsReleased() && eventKey.Keycode == Key.F2)
				{
					Print("Version: " + GameManager.Version);
				}
			}
        }
	}
}
