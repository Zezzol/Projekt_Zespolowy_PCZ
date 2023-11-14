using Godot;
using System;

public partial class Enemy : Area2D
{
    [Signal]
    public delegate void EnemyKilledEventHandler(); //enemy killed signal

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
    bool shootReady = true;
    bool inAnimation = false;

    public override void _Ready()
    {
        label2 = (Label)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/Label2");
        statek = (Statek)GetTree().Root.GetNode("Game/Player/Statek");
        bullet = (PackedScene)ResourceLoader.Load("res://src/EnemyBullet.tscn");
        drop = (PackedScene)ResourceLoader.Load("res://src/UpgradeWeapon.tscn");
        animacja = (AnimationPlayer)GetChild(1);
        wybuch = (AnimatedSprite2D)GetChild(0).GetChild(1);
        sprite = (Sprite2D)GetChild(0).GetChild(0);

        Connect(SignalName.EnemyKilled, new Callable(statek, Statek.MethodName._on_EnemyKilled)); //connect signal

        if (this.Position.X >= 324) //ustaw odpowiednia animacje, zeby przeciwnik nie wylecial poza pole gry
        {
            animationName = "Movement_RightLeft";
        }
        else
        {
            animationName = "Movement_LeftRight";
        }

        Tween t = GetTree().CreateTween();
        t.TweenProperty(this, "position", new Vector2(this.Position.X, this.Position.Y + 100), 1).SetTrans(Tween.TransitionType.Sine); //animacja wlotu na pole gry
    }

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

    public async void PlayMoveAnimation(String animationName)
    {
        inAnimation = true;
        animacja.Play(animationName);
        await ToSignal(animacja, "animation_finished");
        inAnimation = false;
    }

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
            Hit(playerBullet.bulletDmg); //bullet dmg

            body.QueueFree(); //delete bullet after contact
        }
    }

    private void _on_area_entered(Area2D area) //connect body entered signal
    {
        if (area is Statek statek) statek.Hit(contactDmg); //contact dmg with ship
        //dziala tylko jak player wleci w statek, wiec teoretycznie moze w niego wleciec i siedziec tam przez caly czas, a otrzyma obrazenia tylko raz
    }
    public void UpdateScore()
    {
        statek.punkty += 100;
        label2.Text = $"Punkty: {statek.punkty}";
    }
    public async void Hit(int dmg) //take damage
    {
        hp -= dmg;

        if(hp <= 0)
        {
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
