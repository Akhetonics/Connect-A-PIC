using Godot;
using System;

public partial class TutorialSystem : Control
{
    [Export] TextureRect DarkeningArea { get; set; }
    [Export] Sprite2D ExclusionCircle { get; set; }
    [Export] Sprite2D ExclusionSquare { get; set; }



    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }


    public void TutorialOnePorts()
    {
        
    }
}
