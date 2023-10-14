using Godot;
using System;

public partial class EnemyBullet : CharacterBody2D
{
    public Vector2 tor_lotu = Vector2.Zero;
    VisibleOnScreenNotifier2D VisibilityNotifier;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        VisibilityNotifier = (VisibleOnScreenNotifier2D)GetChild(2);
        VisibilityNotifier.ScreenExited += () => QueueFree(); //delete when out of screen
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        tor_lotu.X = 0;
        tor_lotu.Y = (float)(100 * delta); //make speed independent from fps

        MoveAndCollide(tor_lotu);
    }
}
