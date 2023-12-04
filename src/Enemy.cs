using Godot;
using System;

//! @brief Scena pojedynczego przeciwnika.
/*!
  Ta klasa przechowuje logike pojedynczego przeciwnika. 
*/
public partial class Enemy : Area2D
{
    [Signal]
    public delegate void EnemyKilledEventHandler(); /*!< @brief Sygnal pokonania przeciwnika. Emitowany jest do obiektu Statek. */

    [Export] int hp = 10;
    [Export] int contactDmg = 10; //damage on direct contact with the player
    [Export] int bulletDmg = 100; //damage on shooting the player
    PackedScene bullet;
    PackedScene drop;
    AnimationPlayer animacja;
    AnimatedSprite2D wybuch;
    String animationName;
    Statek statek;
    Label label2;
    Sprite2D sprite;
    Tween t;
    bool shootReady = true;
    bool inAnimation = false;

    /*! @brief Funkcja wlaczajaca sie kiedy obiekt zostaje dodany do sceny
    *
    * Podlacza sygnal pokonania przeciwnika do obiektu Statek.
    * Ustawia animacje dla lotu przeciwnika zaleznie od jego wygenerowanej pozycji startowej, a nastepnie ja wlacza.
    */
    public override void _Ready()
    {
        label2 = (Label)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/Label2");
        statek = (Statek)GetTree().Root.GetNode("Game/Player/Statek");
        bullet = (PackedScene)ResourceLoader.Load("res://src/EnemyBullet.tscn");
        drop = (PackedScene)ResourceLoader.Load("res://src/UpgradeWeapon.tscn");
        animacja = (AnimationPlayer)GetChild(1);
        wybuch = (AnimatedSprite2D)GetChild(0).GetChild(1);
        sprite = (Sprite2D)GetChild(0).GetChild(0);
        t = GetTree().CreateTween();

        Connect(SignalName.EnemyKilled, new Callable(statek, Statek.MethodName._on_EnemyKilled)); //connect signal

        if (this.Position.X >= 324) //ustaw odpowiednia animacje, zeby przeciwnik nie wylecial poza pole gry
        {
            animationName = "Movement_RightLeft";
        }
        else
        {
            animationName = "Movement_LeftRight";
        }

        
        t.TweenProperty(this, "position", new Vector2(this.Position.X, this.Position.Y + 100), 1).SetTrans(Tween.TransitionType.Sine); //animacja wlotu na pole gry
    }

    /*! @brief Funkcja wlaczajaca sie w kazdej klatce gry.
    *
    * Wywoluje funkcje Shoot(), jezeli statek jest gotow do strzalu.
    * 
    * Jezeli statek nie jest w animacji, to wlacza ja za pomoca PlayMoveAnimation().
    * 
    * @param delta Oznacza czas ktory minal od ostatniej klatki gry.
    */
    public override void _PhysicsProcess(double delta)
	{
        if (shootReady)
        {
            Shoot();
        }

        if (!inAnimation)
        {
           PlayMoveAnimation(animationName);
        }
    }

    /*! @brief Wlacza animacje.
    * 
    * @param animationName Nazwa animacji.
    */
    public async void PlayMoveAnimation(String animationName)
    {
        inAnimation = true;
        animacja.Play(animationName);
        await ToSignal(animacja, "animation_finished");
        inAnimation = false;
    }

    /*! @brief Wlacza animacje odlotu statku po zakonczeniu gry. */
    public void PlayFlyOffAnimation()
    {
        Tween t2 = GetTree().CreateTween();
        t2.TweenProperty(this, "position", new Vector2(this.Position.X, this.Position.Y + 270), 1).SetEase(Tween.EaseType.In); //animacja wlotu na pole gry
    }

    /*! @brief Strzelanie pociskami przez przeciwnika.
    * 
    * Do sceny dodaje obiekt EnemyBullet i ustawia mu aktualna pozycje przeciwnika.
    * Nastepnie czeka 1 sekunde, aby moc strzelic ponownie.
    */
    public async void Shoot() //spawn bullet
    {
        if (sprite.Visible == true)
        {
            shootReady = false;
            EnemyBullet newBullet = (EnemyBullet)bullet.Instantiate();
            Vector2 newPosition = new Vector2((GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.X, (GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.Y + 17); //get global position of interpolated node
            newBullet.Position = newPosition;
            newBullet.bulletDmg = bulletDmg;
            GetTree().Root.GetNode("Game").AddChild(newBullet);

            await ToSignal(GetTree().CreateTimer(1), "timeout"); //delay beetwen shots
            shootReady = true;
        }
    }

    private void _on_body_entered(Node body) //connect body entered signal
    {
        if (body is PlayerBullet playerBullet)
        {
            if (hp > 0)
            {
                Hit(playerBullet.bulletDmg); //bullet dmg
            }

            body.QueueFree(); //delete bullet after contact
        }
    }

    private void _on_area_entered(Area2D area) //connect body entered signal
    {
        if (area is Statek statek) statek.Hit(contactDmg); //contact dmg with ship
        //dziala tylko jak player wleci w statek, wiec teoretycznie moze w niego wleciec i siedziec tam przez caly czas, a otrzyma obrazenia tylko raz
    }

    /*! @brief Aktualizuje punkty gracza.
    * 
    * Po pokonaniu przeciwnika dodaje 100 punktow do licznika gracza.
    * Licznik punktow nie moze wyniesc wiecej niz 9999.
    */
    public void UpdateScore()
    {
        if (statek.punkty < 9999) statek.punkty += 100;
        label2.Text = $"Punkty: {statek.punkty}";
    }

    /*! @brief Zebranie obrazen od pociskow gracza.
    * 
    * Zmniejsza wartosc punktow zycia przeciwnika i sprawdza, czy sa one mniejsze lub rowne 0.
    * Jezeli tak, to wylacza grafike przeciwnika, wlacza animacje wybuchu i emituje sygnal pokonania przeciwnika do obiektu Statek.
    * Dodaje rowniez obiekt UpgradeWeapon do sceny w miejscu przeciwnika, jezeli Random wylosuje liczbe 0 (20% szans).
    * Na samym koncu usuwa obiekt ze sceny.
    * 
    * @param dmg Wartosc obrazen zadanych przeciwnikowi.
    */
    public async void Hit(int dmg) //take damage
    {
        hp -= dmg;

        if(hp <= 0)
        {
            t.Stop();
            sprite.Visible = false;

            animacja.Pause();

            wybuch.Visible = true;
            wybuch.Play();
            await ToSignal(wybuch, "animation_finished");

            EmitSignal(SignalName.EnemyKilled); //emit enemy killed signal

            this.Visible = false;

            Random rnd = new Random();
            int random = rnd.Next(5); //szansa 20%

            if (random == 0)
            {
                UpgradeWeapon uw = (UpgradeWeapon)drop.Instantiate();
                uw.Position = new Vector2((GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.X, (GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.Y + 17);
                GetTree().Root.GetNode("Game").CallDeferred("add_child", uw);

            }
            UpdateScore();
            this.QueueFree(); //delete after hp = 0
        }
    }
}
