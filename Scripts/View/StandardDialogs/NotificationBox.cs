using Godot;

namespace ConnectAPIC.LayoutWindow.View
{
	public partial class NotificationBox : Control
	{
		private Label messageLabel;
		private Timer timer;
		private Tween tween;
		[Export]private StyleBoxFlat GreenStyle;
		[Export]private StyleBoxFlat RedStyle;
		public override void _Ready()
		{
			messageLabel = GetNode<Label>("Label");
			tween = CreateTween();
			Visible = false;
			tween.TweenProperty(this, "modulate:a",  0, 0f);
			if (GreenStyle == null) GD.PrintErr($"{nameof(GreenStyle)} not attached");
			if (RedStyle == null) GD.PrintErr($"{nameof(RedStyle)} not attached");
		}

		public void ShowMessage(string message, bool isError)
		{
			messageLabel.Text = message;
			
			if (isError)
			{
				AddThemeStyleboxOverride("panel", RedStyle);
			} else
			{
				AddThemeStyleboxOverride("panel", GreenStyle);
			}
			
			Visible = true;
			tween.TweenProperty(this, "modulate:a", 0.9f, 0.25f);
			tween.TweenProperty(this, "modulate:a", 1, 1.25f);
			tween.TweenProperty(this, "modulate:a", 0, 0.25f);
			tween.TweenCallback(Callable.From(QueueFree));
		}

	}
}
