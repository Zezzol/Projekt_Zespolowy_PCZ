using Godot;
using System;

//! @brief Glowna scena gry. 
/*!
  Ta klasa jest glowna scena do ktorej nastepnie dodawane sa wszystkie inne obiekty takie jak Statek, Wave, Enemy itp.
*/
public partial class Game : Node2D
{
	[Export] public int wave = 0; /*!< @brief Numer fali przeciwnikow */
    public int highScore = 0; /*!< @brief Rekord gracza */

    PackedScene waveScene;
	public Wave fala; /*!< @brief Scena Wave, w ktorej generowane sa obiekty Enemy. */

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Sprawdza czy istnieje juz plik zapisu i wczytuje rekord gracza.
    * Jezeli plik nie istnieje, tworzy nowy.
    * 
    * Dodaje do sceny obiekt fali przeciwnikow i rozpoczyna generowanie fal.
    */
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

    /*! @brief Zwieksza numer fali i wywoluje funkcje generowania przeciwnikow
     * 
     * Wywoluje funkcje Wave.AddEnemies() aby wygenerowac przeciwnikow.
    */
    public void StartWave()
	{
		wave += 1; //zwieksz liczbe przeciwnikow
		fala.Call("AddEnemies", wave);
    }

    /*! @brief Funkcja wlaczajaca sie kiedy gracz przegra gre
    *
    * Jezeli gracz uzyskal nowy rekord, zapisuje go do pliku .txt
    * Wyswietla wiadomosc o nowym rekordzie.
    * 
    * Wylacza sterowanie graczowi za pomoca Statek.ChangeProcessMode().
    * 
    * Wywoluje EnemyFlyOffAnimation().
    */
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
		player.GetChild(0).CallDeferred("ChangeProcessMode");

        EnemyFlyOffAnimation();
	}

    /*! @brief Funkcja wlaczajaca animacje odlotu przeciwnikow po koncu gry
    *
    * Funkcja przechodzi w petli po koleji po kazdym przeciwnikow.
    * W kazdej iteracji petli czeka 2 sekundy i wywoluje Enemy.PlayFlyOffAnimation().
    * 
    * Funkcja czeka 2 sekundy w kazdej iteracji aby otrzymac efekt odlatywania przeciwnikow jeden po drugim, zamiast wszyscy na raz.
    */
    public async void EnemyFlyOffAnimation()
	{
        for (int i = 0; i < fala.GetChildCount(); i++)
        {
            await ToSignal(GetTree().CreateTimer(0.2), "timeout");

            var fc = (Enemy)fala.GetChild(i);
            fc.PlayFlyOffAnimation();
        }
    }

    /*! @brief Funkcja wywolujaca sie po kliknieciu ekranu na ekranie przegranej.
     * 
     * Przeladuj scene z gra.
    */
    public void _on_button_pressed()
	{
		GetTree().ReloadCurrentScene();
	}

    /*! @brief Funkcja wywolujaca sie po kliknieciu ekranu na ekranie startowym.
     * 
     * Ukrywa ekran startowy i wywoluje funkcje Statek.Start().
    */
    public void _on_StartButton_pressed()
    {
        Control c = (Control)GetChild(0).GetChild(1);
        c.Hide();

        Node2D player = (Node2D)GetChild(1);
        player.GetChild(0).CallDeferred("Start");
    }

    
}
