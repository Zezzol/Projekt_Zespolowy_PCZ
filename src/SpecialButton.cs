using Godot;
using System;

//! @brief Scena przycisku do wystrzelenia pocisku specjalnego.
public partial class SpecialButton : TouchScreenButton
{
    Node stateknode;
    TextureProgressBar progressbar;
    public int enemyCountRequired = 10; /*!< @brief Wymagana liczba pokonanych przeciwnikow. */
    public int enemyCount = 0; /*!< @brief Liczba aktualnie pokonanych przeciwnikow. */

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
   *
   * Podlacza sygnal klikniecia obiektu przez przeciwnika.
   * Sygnal ten sprawdza, czy liczba pokonanych przeciwnikow zgadza sie z liczba wymaganych pokonanych przeciwnikow.
   * Jezeli tak, to wywoluje funkcje Statek.ShootSpecial().
   * 
   * Aktualizuje rowniez wyglad przycisku.
   */
    public override void _Ready()
    {
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");
        progressbar = (TextureProgressBar)GetChild(0);

        this.Pressed += () =>
        {
            if (enemyCount == enemyCountRequired)
            {
                stateknode.Call("ShootSpecial");
                enemyCount = 0;
                progressbar.Value = 0;
            }
        }; //connect pressed signal
    }
}
