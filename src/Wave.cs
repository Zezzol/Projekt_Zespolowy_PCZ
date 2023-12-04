using Godot;
using System;

//! @brief Klasa odpowiadajaca za generowanie fal przeciwnikow.
public partial class Wave : Node2D
{
    PackedScene enemyScene;

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Ustawia zmienna przechowujaca obiekt sceny pojedynczego przeciwnika.
    */
    public override void _Ready()
	{
        enemyScene = (PackedScene)ResourceLoader.Load("res://src/Enemy.tscn");
    }
	
	/*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Sprawdza czy obiekt ten posiada obiekty przeciwnikow.
    * Jezeli nie, to wywoluje funkcje Game.StartWave().
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
	public override void _Process(double delta)
	{
		if (this.GetChildCount() == 0) //sprawdz czy gracz pokonal wszystkich przeciwnikow w danej fali
		{
			GetParent().Call("StartWave");
		}
	}

    /*! @brief Odpowiada za generowanie przeciwnikow.
    *
    * Na poczatku funkcja sprawdza czy enemyCount jest wieksze od 6.
    * Jezeli tak, to ustawia ten parametr na 6.
    * 
    * W petli funkcja losuje pozycje przeciwnikow i po koleji ich dodaje do sceny jako obiekty podrzedne w tej klasie.
    * 
    * @param enemyCount Parametr oznaczajacy ilosc przeciwnikow do wygenerowania,
    */
    public void AddEnemies(int enemyCount)
	{
		if (enemyCount > 6) enemyCount = 6;

		for (int i = 0; i < enemyCount; i++) //dodaj przeciwnikow
		{
			Enemy enemy = (Enemy)enemyScene.Instantiate();
			Random r = new Random();

			Vector2 enemyPosition = new Vector2(r.Next(100, 548), enemy.Position.Y); //losowa pozycja przeciwnikow
			enemy.Position = enemyPosition;

            AddChild(enemy);
        }
	}
}
