using Godot;
using System;

public partial class GameManager : Node
{
    [Export] public NodePath GridPath { get; set; }
    public Grid Grid { get; set; }
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public override void _Ready()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            QueueFree(); // delete this object as there is already another GameManager in the scene
        }
        Grid = GetNode<Grid>(GridPath);
    }
}
