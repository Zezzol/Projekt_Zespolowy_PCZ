using Godot;
using System;

//! @brief Obiekt ulepszen zbieranych przez statek.
/*!
  Klasa ta decyduje o tym jak ulepszyc statek na podstawie zebranych ulepszen.
  Moze ona ulepszyc i odnowic punkty zycia przeciwnika, lub zmienic tryb strzelania.
*/
public partial class UpgradeWeapon : Node2D
{
	int upgradeStat; //0 = upgrade HP, 1 = upgrade broni

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Decyduje o tym, co obiekt bedzie ulepszac, zycie czy strzelanie i zapisuje to do zmiennej.
    *
    * Ustawia kolor ulepszenia w zaleznosci od tego, co bedzie ten obiekt ulepszac.
    */
    public override void _Ready()
	{
		Random rnd = new Random();
		upgradeStat = rnd.Next(2);
		
		if (upgradeStat == 0 )
		{
			Modulate = new Color("00ff00");
		}
	}

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Odpowiada za poruszanie sie ulepszenia w dol ekranu.
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
    public override void _Process(double delta)
	{
		Position += new Vector2(0, (float)(50 * delta));
	}

    /*! @brief Funkcja wlaczajaca sie, jezeli obiekt wykryje kolizje.
    *
    * Sprawdza czy kolizja nastapila z obiektem Statek. 
    * Jezeli tak, to w zaleznosci od zmiennej w ktorej zapisane jest, co obiekt ma ulepszac ustawiane sa odpowienio statystyki.
    * Jezeli obiekt ma ulepszac zycie, to ustawia wartosci punktow zycia obiektu Statek na maksymalna wartosc, a nastepnie zwieksza ja o 50.
    * Jezeli ma ulepszac strzelanie, wywoluje funkcje Statek.Upgrade().
    * Na samym koncu usuwa obiekt.
    * 
    * @param area Obiekt z ktorym wykryto kolizje.
    */
    public void _on_area_entered(Area2D area)
	{
        if (area is Statek statek)
        {
            if (upgradeStat == 0)
            {
				statek.maksHp += 50;
				statek.hp = statek.maksHp;
				statek.hpBar.MaxValue = statek.maksHp;
				statek.hpBar.Value = statek.hp;
            }
			else
			{
				if (statek.liczbaPociskow < 3)
				{
					statek.liczbaPociskow++;
					statek.Upgrade();
				}
			}

			this.QueueFree();
        }
	}
}
