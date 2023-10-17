using Godot;
using System;

public partial class Game : Node2D
{
	[Export] public int wave = 0; //numer fali i liczba przeciwnikow

	PackedScene waveScene;
	Wave fala;

	public override void _Ready()
	{
		waveScene = (PackedScene)ResourceLoader.Load("res://src/Wave.tscn");
		fala = (Wave)GetTree().Root.GetNode("Game/Wave");
		fala.ProcessMode = ProcessModeEnum.Disabled;
    }

	public void StartWave()
	{
		wave += 1; //zwieksz liczbe przeciwnikow
		fala.Call("AddEnemies", wave);
    }

	public void GameOver() //game over, wylacz sterowanie graczowi
	{
		Control c = (Control)GetChild(0).GetChild(0);
		c.Show();

		Node2D player = (Node2D)GetChild(1);
		player.Hide();
		player.GetChild(0).CallDeferred("ChangeProcessMode");
	}

    public void _on_button_pressed() //restart
	{
		GetTree().ReloadCurrentScene();
	}

    public void _on_StartButton_pressed() //po kliknieciu start, wlacz sterowanie graczowi, zespawnuj przeciwnikow
    {
        Control c = (Control)GetChild(0).GetChild(1);
        c.Hide();

        Node2D player = (Node2D)GetChild(1);
        player.GetChild(0).CallDeferred("ChangeProcessMode");

		fala.ProcessMode = ProcessModeEnum.Inherit;
		StartWave();
    }
}
