using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Statek : CharacterBody2D
{
    [Export] int hp = 10;
    [Export] int bulletDmg = 5; //damage on shooting the player

    private Vector2 joysticksygnal;
    private Vector2 statekruch = Vector2.Zero;
    PackedScene pocisk;
    ShootButton przyciskDoStrzelania;
    SpecialButton przyciskDoSpeciala;
    AnimationPlayer animacja;
    public override void _Ready()
    {
        pocisk = (PackedScene)ResourceLoader.Load("res://src/PlayerBullet.tscn");
        przyciskDoStrzelania = (ShootButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/ShootButton");
        przyciskDoSpeciala = (SpecialButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/SpecialButton");
        animacja = (AnimationPlayer)GetChild(2);
        
    }
    public override void _PhysicsProcess(double delta)
    {
        statekruch.X = (float)((joysticksygnal.X * 100) * delta); //make speed indepentend from fps
        statekruch.Y = (float)((joysticksygnal.Y * 100) * delta);
        MoveAndCollide(statekruch);
    }
    private void ReciveJoystick(Vector2 recivelJoystick)
    {
        joysticksygnal = recivelJoystick;
    }

    //shooting scripts
    public async void Shoot() //Wygeneruj pocisk, ustaw tor lotu i wystrzel
    {
        PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        nowyPocisk.bulletDmg = bulletDmg;
        GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);

        await ToSignal(GetTree().CreateTimer(0.2), "timeout"); //delay beetwen shots
        przyciskDoStrzelania.shootReady = true;
    }
    
    public void ShootSpecial()
    {
        PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        nowyPocisk.Scale = new Vector2(4, 4);
        GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
        przyciskDoSpeciala.shootReady = false;
    }

    //getting damaged scripts
    public async void Hit(int dmg)
    {
        hp -= dmg;

        if (hp <= 0) GameOver(); //lose the game
        else
        {
            animacja.Play("Hit");
            await ToSignal(animacja, "animation_finished");
        }
    }

    public void GameOver()
    {
        GD.Print("Ded");
    }
}
