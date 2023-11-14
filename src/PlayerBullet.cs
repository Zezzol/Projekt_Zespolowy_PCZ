using Godot;
using System;

public partial class PlayerBullet : CharacterBody2D
{
	public Vector2 tor_lotu = Vector2.Zero;
    public int bulletDmg = 0;
	public int speed = -100;
    VisibleOnScreenNotifier2D VisibilityNotifier;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VisibilityNotifier = (VisibleOnScreenNotifier2D)GetChild(2);
		VisibilityNotifier.ScreenExited += () => QueueFree(); //delete when out of screen

		//GD.Print("piu");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tor_lotu.X = 0;
		tor_lotu.Y = (float)(speed * delta); //make speed independent from fps

        if (speed == -150)
		{
            var x = (CollisionShape2D)GetChild(1);
            x.Scale = new Vector2(2, 2);

            var y = (Sprite2D)GetChild(0);
			y.Visible = false;

			var z = (Sprite2D)GetChild(3);
			z.Visible = true;
		}

        MoveAndCollide(tor_lotu);
	}
}
