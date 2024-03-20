using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.CodeDom;
using System.Linq;

namespace ConnectAPIC.Scenes.RightClickMenu
{
    public partial class ControlMenu : CharacterBody2D
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
            };
            area.MouseShapeExited += (long shapeIdx) =>
            {
                mouseIn = false;
            };
        }
        public override void _Input(InputEvent @event)
        {
            //dragging
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.IsPressed() && mouseIn)
                {
                    draggingDistance = Position.DistanceTo(GetViewport().GetMousePosition());
                    direction = (GetViewport().GetMousePosition() - Position).Normalized();
                    dragging = true;
                    newPosition = GetViewport().GetMousePosition() - draggingDistance * direction;
                }
                else
                {
                    dragging = false;
                }
            }
            else if (@event is InputEventMouseMotion mouseMotion && dragging)
            {
                newPosition = GetViewport().GetMousePosition() - draggingDistance * direction;
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

        public T AddSection<T>() where T : ISection
        {
            PackedScene packedSection = GetSectionTemplate<T>();
            if (packedSection == null) return null;

            T section = packedSection.Instantiate<T>();
            sectionContainer.AddChild(section);
            return section;
        }
        public T RemoveSection<T>() where T : ISection
        {
            foreach (Node node in sectionContainer.GetChildren())
            {
                ISection item = node as ISection;
                if (item != null && item.GetType() == typeof(T))
                {
                    sectionContainer.RemoveChild(node);
                    return (T)item;
                }
            }
            return null;
        }
        private PackedScene GetSectionTemplate<T>() where T : ISection
        {
            if (typeof(T) == typeof(SliderSection))
                return SliderSectionTemplate;
            else if (typeof(T) == typeof(ToggleSection))
                return ToggleSectionTemplate;
            else if (typeof(T) == typeof(OnOffSection))
                return OnOffSectionTemplate;
            else if (typeof(T) == typeof(InfoSection))
                return InfoSectionTemplate;
            else return null;
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

        private void OnCloseButtonPressed()
        {
            this.Visible = false;
        }
    }
}
