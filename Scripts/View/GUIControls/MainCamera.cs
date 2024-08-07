using CAP_Contracts.Logger;
using Chickensoft.AutoInject;
using ConnectAPIC.Scenes.InteractionOverlay;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using JetBrains.Annotations;
using SuperNodes.Types;
using System;

[SuperNode(typeof(Dependent))]
public partial class MainCamera : Camera2D
{
    public override partial void _Notification(int what);
    [Dependency] public ILogger Logger => DependOn<ILogger>();
    [Export] public Node2D CenteringPoint {  get; set; }

    [Export] public float InitialZoom { get; set; } = 1;
    [Export] public float ZoomSpeed { get; set; } = 0.1f;
    [Export] public float MinZoomIn { get; set; } = 0.5f;
    [Export] public float MaxZoomIn { get; set; } = 2.7f;
    [Export] public float PanSensitivity { get; set; } = 1f;

    public bool autoCenterWhenResizing = false;

    public bool noZoomingOrMoving = false;


    public override void _UnhandledInput(InputEvent @event)
    {
        if (noZoomingOrMoving) return;

        if(@event is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Right) && InteractionOverlayController.ClickingAllowed)
        {
            Position -= ((InputEventMouseMotion)@event).Relative * PanSensitivity / Zoom;
        }

        if(@event is InputEventMouseButton && InteractionOverlayController.ScrollingAllowed)
        {
            if (((InputEventMouseButton)@event).ButtonIndex == MouseButton.WheelUp)
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
        if (autoCenterWhenResizing) RecenterCamera();

        Zoom = new Vector2(InitialZoom, InitialZoom);

        GetViewport().SizeChanged += () =>
        {
            if (autoCenterWhenResizing)
            {
                RecenterCamera();
            }
        };
    }

    public void OnResolved()
    {
        this.CheckForNull((c) => c.CenteringPoint, Logger);
    }

    public void RecenterCamera()
    {
        ResetCameraZoom();
        var X = GetViewport().GetVisibleRect().Size.X;
        var Y = GetViewport().GetVisibleRect().Size.Y;
        Position = CenteringPoint.GlobalPosition - new Vector2(X / 2, Y / 2);
    }

    private void ResetCameraZoom()
    {
        Zoom = new Vector2(InitialZoom, InitialZoom);
    }

    public void ZoomCamera(int direction){
        if (noZoomingOrMoving) return;
        Vector2 previousMousePosition = GetLocalMousePosition();
        
        Zoom += Zoom * ZoomSpeed * direction;
        Zoom = Zoom.Clamp(new Vector2(MinZoomIn, MinZoomIn), new Vector2(MaxZoomIn, MaxZoomIn));
            
        Position += previousMousePosition - GetLocalMousePosition();
    }

    private void OnMouseEnteredTutorialWindow() {
        noZoomingOrMoving = true;
    }
    private void OnMouseExitedTutorialWindow() {
        noZoomingOrMoving = false;
    }
}

