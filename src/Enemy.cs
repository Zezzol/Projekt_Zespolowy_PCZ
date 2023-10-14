using Godot;
using System;

public partial class Enemy : Area2D
{
    [Export] int hp = 10;
    [Export] int contactDmg = 10; //damage on direct contact with the player
    [Export] int bulletDmg = 5; //damage on shooting the player
    PackedScene bullet;
    AnimationPlayer animacja;

    bool shootReady = true;
    bool inAnimation = false;

    public override void _Ready()
    {
        bullet = (PackedScene)ResourceLoader.Load("res://src/EnemyBullet.tscn");
        animacja = (AnimationPlayer)GetChild(1);
    }

    public override void _PhysicsProcess(double delta)
	{
        if (shootReady)
        {
            Shoot();
        }

        if (!inAnimation)
        {
            PlayMoveAnimation();
        }
    }

    public async void PlayMoveAnimation()
    {
        inAnimation = true;
        animacja.Play("Movement_LeftRight");
        await ToSignal(animacja, "animation_finished");
        inAnimation = false;
    }

    public async void Shoot() //spawn bullet
    {
        shootReady = false;
        EnemyBullet newBullet = (EnemyBullet)bullet.Instantiate();
        Vector2 newPosition = new Vector2((GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.X, (GetNode("CollisionPolygon2D") as Node2D).GlobalPosition.Y + 17); //get global position of interpolated node
        newBullet.Position = newPosition;
        GetTree().Root.GetNode("Game").AddChild(newBullet);

        await ToSignal(GetTree().CreateTimer(1), "timeout"); //delay beetwen shots
        shootReady = true;
    }

    private void _on_body_entered(Node body) //connect body entered signal
    {
        if (body is Statek statek) statek.Hit(contactDmg);
        else if (body is PlayerBullet playerBullet)
        {
            Hit(playerBullet.bulletDmg);
            body.QueueFree(); //delete bullet after contact
        }
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
