using Godot;
using System;

//! @brief Obiekt pocisku gracza.
/*!
  Ta klasa przechowuje tor lotu pocisku, oraz obrazenia zadane nim.
  Posiada tez VisibilityNotifier, aby wiadomo bylo, kiedy pocisk wyszedl poza ekran.
*/
public partial class PlayerBullet : CharacterBody2D
{
	public Vector2 tor_lotu = Vector2.Zero; /*!< @brief Tor lotu pocisku. */
    public int bulletDmg = 0; /*!< @brief Wartosc obrazen. */
    public int speed = -100; /*!< @brief Szybkosc pocisku. */
    VisibleOnScreenNotifier2D VisibilityNotifier;

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Ustawia VisibilityNotifier. Jezeli gra wykryje, ze pocisk jest poza ekranem gry, to usunie go.
    */
    public override void _Ready()
	{
		VisibilityNotifier = (VisibleOnScreenNotifier2D)GetChild(2);
		VisibilityNotifier.ScreenExited += () => QueueFree(); //delete when out of screen
	}

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Funkcja ta, odpowiada za poruszanie pociskiem po domyslnym torze lotu.
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
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
