using Godot;
using System;
using System.Text.RegularExpressions;

//! @brief Klasa statku gracza.
/*!
  Ta klasa przechowuje logike dotyczaca statku gracza, ilosc punktow zycia, obrazenia zadawane od pociskow itp.

  Przechowuje rowniez i zlicza punkty zdobyte przez gracza.
*/
public partial class Statek : Area2D
{
    [Export] public int punkty = 0; /*!< @brief Liczba aktualnych zdobytych punktow. */
    [Export] public int maksHp = 100; /*!< @brief Maksymalna wartosc punktow zycia. */
    [Export] public int hp = 100; /*!< @brief Aktualna wartosc punktow zycia. */
    [Export] int bulletDmg = 5; //damage on shooting the player
    [Export] public int liczbaPociskow = 1; /*!< @brief Liczba pociskow, ktore gracz moze na raz wystrzelic. */

    private bool iframes = false; //invincibility

    private Vector2 joysticksygnal;
    private Vector2 statekruch = Vector2.Zero;
    PackedScene pocisk;
    ShootButton przyciskDoStrzelania;
    SpecialButton przyciskDoSpeciala;
    public TextureProgressBar hpBar; /*!< @brief Obiekt pasku zycia pokazujacego aktualne punkty zycia gracza. */
    public AnimationPlayer animacja; /*!< @brief Obiekt AnimationPlayer, za pomoca ktorego wykonywane sa animacje gracza. */
    AnimatedSprite2D sprite;

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Ustawia zmienne przechowujace obiekty takie jak, pociski, przyciski i grafike.
    */
    public override void _Ready()
    {
        pocisk = (PackedScene)ResourceLoader.Load("res://src/PlayerBullet.tscn");
        przyciskDoStrzelania = (ShootButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/ShootButton");
        przyciskDoSpeciala = (SpecialButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/SpecialButton");
        hpBar = (TextureProgressBar)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/HPBar");
        animacja = (AnimationPlayer)GetChild(2);
        sprite = (AnimatedSprite2D)GetTree().Root.GetNode("Game/Player/Statek/Statek_kadlub");
    }

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Pobiera wartosci od Joystick, ktore nastepnie sa konwertowane na wektor ruchu.
    * 
    * Zawraca statek spowrotem na pole gry, jezeli poza nie wyleci.
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
    public override void _PhysicsProcess(double delta)
    {
        
        statekruch.X = (float)((joysticksygnal.X * 100) * delta); //make speed indepentend from fps
        statekruch.Y = (float)((joysticksygnal.Y * 100) * delta);

        if(Position.X >= GetViewportRect().Size.X)
        {
            Position = new Vector2(Position.X - 1, Position.Y);
        }
        if (Position.Y >= GetViewportRect().Size.Y)
        {
            Position = new Vector2(Position.X, Position.Y - 1);
        }
        if (Position.X <= 0)
        {
            Position = new Vector2(Position.X + 1, Position.Y);
        }
        if (Position.Y <= 0)
        {
            Position = new Vector2(Position.X, Position.Y + 1);
        }
        if (statekruch != Vector2.Zero) Position += statekruch;
    }

    /*! @brief Funkcja odbierajaca wartosci z Joystick potrzebne do wyliczenia wektora ruchu.
    * 
    * @param recivelJoystick Parametr z wartosciami z Joystick.
    */
    private void ReciveJoystick(Vector2 recivelJoystick)
    {
        joysticksygnal = recivelJoystick;
    }

    /*! @brief Strzelanie pociskami przez gracza.
    * 
    * Do sceny dodaje obiekt playerBullet i ustawia mu odpowiednia pozycje.
    * Nastepnie czeka, aby moc strzelic ponownie.
    * 
    * Sprawdza rowniez ilosc przyciskow, ktore gracz moze wystrzelic na raz i dodaje odpowiednia ich liczbe w zaleznosci od zmiennej liczbaPociskow.
    */
    public async void Shoot() //Wygeneruj pocisk, ustaw tor lotu i wystrzel
    {
        switch (liczbaPociskow) {//do poprawienia
            case 1:
                {
                    PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
                    nowyPocisk.bulletDmg = bulletDmg;
                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
                }
                break;
            case 2:
                {
                    PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk.Position = new Vector2(this.Position.X - 10, this.Position.Y - 17);
                    nowyPocisk.bulletDmg = bulletDmg;

                    PlayerBullet nowyPocisk2 = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk2.Position = new Vector2(this.Position.X + 10, this.Position.Y - 17);
                    nowyPocisk2.bulletDmg = bulletDmg;

                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk2);
                }
                break;
            case 3:
                {
                    PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk.Position = new Vector2(this.Position.X - 15, this.Position.Y - 17);
                    nowyPocisk.bulletDmg = bulletDmg;

                    PlayerBullet nowyPocisk2 = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk2.Position = new Vector2(this.Position.X + 15, this.Position.Y - 17);
                    nowyPocisk2.bulletDmg = bulletDmg;

                    PlayerBullet nowyPocisk3 = (PlayerBullet)pocisk.Instantiate();
                    nowyPocisk3.Position = new Vector2(this.Position.X , this.Position.Y - 17);
                    nowyPocisk3.bulletDmg = bulletDmg;

                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk2);
                    GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk3);
                }
                break;
        }
        

        await ToSignal(GetTree().CreateTimer(0.2), "timeout"); //delay beetwen shots
        przyciskDoStrzelania.shootReady = true;
    }

    /*! @brief Strzelanie pociskami specjalnymi.
    * 
    * Do sceny dodaje obiekt playerBullet i ustawia mu odpowiednia pozycje.
    * Zwieksza mu domyslne obrazenia, predkosc i skale.
    */
    public void ShootSpecial()
    {
        PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        nowyPocisk.Scale = new Vector2(4, 4);
        nowyPocisk.bulletDmg = 100;
        nowyPocisk.speed = -150;
        GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
    }

    /*! @brief Wywoluje sie, jezeli Statek wykryje kolizje z innym obiektem.
    * 
    * Sprawdza czy tym obiektem jest EnemyBullet, jezeli tak to wywoluje metode Hit(), a nastepnie usuwa obiekt EnemyBullet.
    * 
    * @param body Zawiera obiekt z ktorym nastopila kolizja.
    */
    private void _on_body_entered(Node body) //connect body entered signal
    {
        if (body is EnemyBullet enemyBullet)
        {
            Hit(enemyBullet.bulletDmg);
            body.QueueFree(); //delete bullet after contact
        }
    }

    /*! @brief Zbieranie obrazen przez gracza.
    * 
    * Sprawdza, czy gracz aktualnie jest nietykalny.
    * Jezeli nie, to wlacza nietykalnosc i zmniejsza ilosc punktow zycia o odpowiednia wartosc i animuje pasek zycia gracza.
    * Nastepnie sprawdza, czy ilosc punktow zycia gracza jest mniejsza lub rowna 0.
    * Jezeli tak, to grafika statku i interfejs sa wylaczane, wlaczana jest animacja wybuchu i wywolywana jest funkcja Game.GameOver().
    * Jezeli nie, to wlaczana jest animacja otrzymania obrazen i wylaczana jest nietykalnosc.
    * 
    * @param dmg Ilosc otrzymanych obrazen.
    */
    public async void Hit(int dmg)
    {
        if (!iframes)
        {
            iframes = true;
            hp -= dmg;

            Tween t = CreateTween();
            t.TweenProperty(GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/HPBar"), "value", hp, 0.2f);

            if (hp <= 0) {
                var canvas = (CanvasLayer)GetParent().GetChild(1);
                canvas.Visible = false;

                sprite.Play("end");
                await ToSignal(sprite, "animation_finished");
                sprite.Visible = false;                

                GetTree().Root.GetNode("Game").Call("GameOver"); //lose the game
            } 
            else
            {
                animacja.Play("Hit");
                await ToSignal(animacja, "animation_finished");
                iframes = false;
            }
        }
    }

    /*! @brief Zmienia tryb procesowania obiektu Statek na przeciwny do aktualnego (wlacza i wylacza sterowanie). */
    public void ChangeProcessMode()
    {
        if (GetParent().ProcessMode == ProcessModeEnum.Disabled) GetParent().ProcessMode = ProcessModeEnum.Inherit;
        else GetParent().ProcessMode = ProcessModeEnum.Disabled;
    }

    /*! @brief Zmienia ilosc przyciskow wystrzeliwanych przez gracza. 
    *
    * Wlacza rowniez animacje ulepszenia statku i przelacza grafike statku.
    */
    public async void Upgrade()
    {
        if (liczbaPociskow == 2)
        {
            sprite.Play("level_up1");
            await ToSignal(sprite, "animation_finished");
            sprite.Play("second_stage");
        }
        else if (liczbaPociskow == 3)
        {
            sprite.Play("level_up2");
            await ToSignal(sprite, "animation_finished");
            sprite.Play("third_stage");
        }
    }

    /*! @brief Funkcja wywolujaca sie w odpowiedzi na sygnal zabicia przeciwnika. 
    *
    * Zwieksza ilosc zmienna SpecialButton.enemyCount.
    */
    public void _on_EnemyKilled()
    {
        GD.Print(przyciskDoSpeciala.enemyCount);
        if (przyciskDoSpeciala.enemyCount != przyciskDoSpeciala.enemyCountRequired)
        {
            przyciskDoSpeciala.enemyCount += 1;
            var x = (TextureProgressBar)przyciskDoSpeciala.GetChild(0);
            x.Value += 10;
        }
    }

    /*! @brief Funkcja wywolywana po starcie nowej gry. 
    *
    * Zmienia tryb procesowania za pomoca ChangeProcessMode() i wlacza animacje wlotu gracza na pole gry.
    * Nastepnie wlacza interfejs uzytkownika.
    */
    public async void Start()
    {
        ChangeProcessMode();

        animacja.Play("Start");
        await ToSignal(animacja, "animation_finished");

        var x = (CanvasLayer)GetParent().GetChild(1);
        x.Visible = true;

        var y = (Game)GetTree().Root.GetNode("Game");
        y.fala.ProcessMode = ProcessModeEnum.Inherit;
    }
}
