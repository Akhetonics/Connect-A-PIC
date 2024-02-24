using Godot;
using System;

public partial class MainCamera : Camera2D
{
    [Export] public float InitialZoom { get; set; } = 1;
    [Export] public float ZoomSpeed { get; set; } = 0.1f;
    [Export] public float MinZoomIn { get; set; } = 0.5f;
    [Export] public float MaxZoomIn { get; set; } = 2.7f;
    [Export] public float PanSensitivity { get; set; } = 1f;


    public override void _Input(InputEvent @event)
    {
        
        if(@event is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Right))
        {
            Position -= ((InputEventMouseMotion)@event).Relative * PanSensitivity / Zoom;
        }

        if(@event is InputEventMouseButton)
        {
            if( ((InputEventMouseButton)@event).ButtonIndex == MouseButton.WheelUp)
            {
                ZoomCamera(1);
            }
            else if (((InputEventMouseButton)@event).ButtonIndex == MouseButton.WheelDown)
            {
                ZoomCamera(-1);
            }
        }
    }

    public override void _Ready()
    {
        Zoom = new Vector2(InitialZoom, InitialZoom);
    }
    
    
    public void ZoomCamera(int direction){
        Vector2 previousMousePosition = GetLocalMousePosition();
        
        Zoom += Zoom * ZoomSpeed * direction;
        Zoom = Zoom.Clamp(new Vector2(MinZoomIn, MinZoomIn), new Vector2(MaxZoomIn, MaxZoomIn));
        
        Offset += previousMousePosition - GetLocalMousePosition();
    }
}
