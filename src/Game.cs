using Godot;
using System;

public partial class Game : Node2D
{
	[Export] public int wave = 0; //numer fali i liczba przeciwnikow
	public int highScore = 0;

	PackedScene waveScene;
	Wave fala;

	public override void _Ready()
	{
        if (FileAccess.FileExists("user://save.txt"))
        {
            var save = FileAccess.Open("user://save.txt", FileAccess.ModeFlags.Read);
			var x = save.GetLine();
			highScore = x.ToInt();
            GD.Print(highScore);
			save.Close();
        }

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
		Label score = (Label)c.GetChild(2);
        Label score2 = (Label)c.GetChild(3);

        Statek statek = (Statek)GetNode("Player").GetChild(0);

        if (statek.punkty > highScore)
		{
			highScore = statek.punkty;
			score.Visible = true;

            var save = FileAccess.Open("user://save.txt", FileAccess.ModeFlags.Write);
            save.StoreLine(highScore.ToString());
            save.Close();
        }
		else
		{
            score.Visible = false;
        }
        score2.Text = "Rekord: " + highScore;

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
