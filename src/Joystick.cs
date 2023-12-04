using Godot;
using System;

//! @brief Obiekt odpowiadajacy za jostick do poruszania statkiem.
/*!
  Klasa ta zbiera dane z joysticka, o tym, gdzie gracz chce pokierowac statkiem i przesyla je do klasy Statek.
*/
public partial class Joystick : TouchScreenButton
{
    [Export]
    private int granica = 48;
    [Export]
    private int ignore = 150;
    private Vector2 buttonSize;
    private Vector2 buttonPosition;
    Node stateknode;

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Ustawia zmienne przechowujace rozmiar przycisku.
    */
    public override void _Ready()
    {
        buttonSize = new Vector2(TexturePressed.GetWidth(), TexturePressed.GetHeight());
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");

    }

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Pobiera wartosci z joysticka, jezeli jest wcisniety, i wysyla je do obiektu Statek poprzez Statek.reciveJoystick().
    * Jezeli nie, to ustawia wartosci na 0.
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
    public override void _Process(double delta)
    {
        if (!IsPressed())
        {
            Position = Vector2.Zero;
        }

        if (stateknode != null && stateknode.IsInsideTree())
        {
            stateknode.Call("ReciveJoystick", Position / granica);
        }
    }

    /*! @brief Funkcja wlaczajaca sie kiedy gra wykryje wprowadzenie danych od gracza.
   *
   * Sprawdzane jest wydarzenie.
   * Jezeli wydarzenie jest wcisnieciem przycisku joysticka, to sprawdza czy gracz wciska przycisk w polu pocisku.
   * Jezeli tak, to zapisuje dane o pozycji przycisku.
   * 
   * @param inputEvent Jest to wydarzenie, ktore gra wykryje.
   */
    public override void _Input(InputEvent inputEvent)
    {
        if (!IsPressed())
        {
            Position = Vector2.Zero;
        }
   
        if(inputEvent is InputEventScreenDrag eventScreenDrag)
        {
            if((eventScreenDrag.Position - GlobalPosition).Length()>ignore)
            {
                return;
            }
            this.GlobalPosition = eventScreenDrag.Position - buttonSize / 2;
         
            if (Position.Length()  > granica)
            {
                Position=(Position.Normalized()) * granica;
            }
        }
    }
   
}
