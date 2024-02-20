using Godot;
using System;

public partial class MainCamera : Camera2D
{
	[Export] public float InitialZoom { get; set; } = 1;
	[Export] public float ZoomSpeed { get; set; } = 0.05f;
	[Export] public float MinZoomIn { get; set; } = 0.8f;
	[Export] public float MaxZoomIn { get; set; } = 2f;
	[Export] public float PanSensitivity { get; set; } = 1f;


	public override void _Input(InputEvent @event)
	{
		
		if(@event is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Right))
		{
			Position -= ((InputEventMouseMotion)@event).Relative * PanSensitivity / Zoom;
		}

		if(@event is InputEventMouseButton)
		{
			Vector2 zoomSpd = new Vector2(ZoomSpeed, ZoomSpeed);
			Vector2 minZoom = new Vector2(MinZoomIn, MinZoomIn);
			Vector2 maxZoom = new Vector2(MaxZoomIn, MaxZoomIn);

			if( ((InputEventMouseButton)@event).ButtonIndex == MouseButton.WheelUp)
			{
				Zoom += zoomSpd;
			}
			else if (((InputEventMouseButton)@event).ButtonIndex == MouseButton.WheelDown)
			{
				Zoom -= zoomSpd;
			}

			Zoom = Zoom.Clamp(minZoom, maxZoom);
		}
	}

	public override void _Ready()
	{
		Zoom = new Vector2(InitialZoom, InitialZoom);
	}
}
