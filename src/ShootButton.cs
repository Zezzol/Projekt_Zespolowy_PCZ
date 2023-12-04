using Godot;
using System;
using System.Reflection.Metadata;

//! @brief Obiekt przycisku gracza do strzelania pociskow.
public partial class ShootButton : TouchScreenButton
{
    Node stateknode;
    bool shooting = false;
    public bool shootReady = true; /*!< @brief Przechowuje informacje o mozliwosci strzalu. */

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Ustawia dwa sygnaly: Pressed i Released.
    * 
    * Sygnal Pressed oznacza, ze gracz aktualnie strzela, a sygnal Released oznacza brak strzelania.
    */
    public override void _Ready()
	{
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");
        this.Pressed += () => shooting = true; //connect pressed signal
        this.Released += () => shooting = false; //connect released signal
    }

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Sprawdza czy gracz aktualnie jest w trakcie strzelania i wywoluje funkcje Statek.Shoot().
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
    public override void _Process(double delta)
	{
        if (shootReady && shooting)
        {
            stateknode.Call("Shoot");
            shootReady = false;
        }
	}
}
