using Godot;
using System;

public partial class Game : Node2D
{
	public void GameOver()
	{
		CanvasLayer cl = (CanvasLayer)GetChild(0);
		cl.Show();

		Node2D player = (Node2D)GetChild(1);
		player.GetChild(0).CallDeferred("DisableProcessMode");
	}

    public void _on_button_pressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
