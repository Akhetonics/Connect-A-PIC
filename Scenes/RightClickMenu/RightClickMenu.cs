using Godot;
using System;
using System.Diagnostics;

public partial class RightClickMenu : CharacterBody2D
{
    bool dragging;
    bool mouseIn = false;
    Vector2 direction;
    Vector2 newPosition;
    float draggingDistance;
    float draggingSpeed = 30;

    Area2D area;

    public override void _Ready()
    {
        area = GetChild<Area2D>(1);
        area.MouseEntered += () =>
        {
            mouseIn = true;
            Debug.Print("mouse entered");
        };

        area.MouseExited += () =>
        {
            mouseIn = false;
            Debug.Print("mouse exited");
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton) {
            if (mouseButton.IsPressed() && mouseIn) {
                draggingDistance = Position.DistanceTo(GetViewport().GetMousePosition());
                direction = (GetViewport().GetMousePosition() - Position).Normalized();
                dragging = true;
                newPosition = GetViewport().GetMousePosition() - Position - draggingDistance*direction;
            }
            else
            {
                dragging = false;
            }
        }
        else if (@event is InputEventMouseMotion mouseMotion && dragging)
        {
            newPosition = GetViewport().GetMousePosition() - Position - draggingDistance * direction;
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (dragging)
        {
            Velocity = (newPosition - Position) * (new Vector2(draggingSpeed, draggingSpeed));
            MoveAndSlide();
        }
    }


    private void MouseEnteredInDragArea()
    {
        mouseIn = true;
        Debug.Print("mouse entered");
    }


    private void MouseExitedDragArea()
    {
        mouseIn = false;
        Debug.Print("mouse exited");
    }


}



