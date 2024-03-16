using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class RightClickMenu : CharacterBody2D
{
    [Export] public PackedScene ToggleSectionTemplate { get; set; }
    [Export] public PackedScene OnOffSectionTemplate { get; set; }
    [Export] public PackedScene SliderSectionTemplate { get; set; }
    [Export] public PackedScene InfoSectionTemplate { get; set; }

    VBoxContainer sectionContainer { get; set; }


    //dragging
    bool dragging;
    bool mouseIn = false;
    Vector2 direction;
    Vector2 newPosition;
    float draggingDistance;
    float draggingSpeed = 50;

    Area2D area;

    public override void _Ready()
    {
        sectionContainer = GetNode<VBoxContainer>("%SectionContainer");

        //dragging
        area = GetChild<Area2D>(1);
        area.MouseShapeEntered += (long shapeIdx) =>
        {
            mouseIn = true;
            Debug.Print("mouse entered");
        };
        area.MouseShapeExited += (long shapeIdx) => {
            mouseIn = false;
            Debug.Print("mouse exited");
        };
    }

    public override void _Input(InputEvent @event)
    {
        //dragging
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


    public RightClickMenu AddToggleSection()
    {
        = ToggleSectionTemplate.Instantiate();

        return this;
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



