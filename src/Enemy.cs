using Godot;
using System;

public partial class Enemy : Area2D
{
    [Export] int hp = 10;
    [Export] int contactDmg = 10; //damage on direct contact with the player
    [Export] int bulletDmg = 100; //damage on shooting the player
    PackedScene bullet;
    AnimationPlayer animacja;
    String animationName;

    bool shootReady = true;
    bool inAnimation = false;

    public override void _Ready()
    {
        bullet = (PackedScene)ResourceLoader.Load("res://src/EnemyBullet.tscn");
        animacja = (AnimationPlayer)GetChild(1);

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
        shootReady = false;
        EnemyBullet newBullet = (EnemyBullet)bullet.Instantiate();
        Vector2 newPosition = new Vector2((GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.X, (GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.Y + 17); //get global position of interpolated node
        newBullet.Position = newPosition;
        newBullet.bulletDmg = bulletDmg;
        GetTree().Root.GetNode("Game").AddChild(newBullet);

        await ToSignal(GetTree().CreateTimer(1), "timeout"); //delay beetwen shots
        shootReady = true;
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

    public void Hit(int dmg) //take damage
    {
        hp -= dmg;

        if(hp <= 0)
        {
            this.Visible = false;
            this.QueueFree(); //delete after hp = 0
        }
    }
}
