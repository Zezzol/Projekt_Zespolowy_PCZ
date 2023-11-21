using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Statek : Area2D
{
    [Export] public int punkty = 0;
    [Export] public int maksHp = 100;
    [Export] public int hp = 100;
    [Export] int bulletDmg = 5; //damage on shooting the player
    [Export] public int liczbaPociskow = 1;

    private bool iframes = false; //invincibility

    private Vector2 joysticksygnal;
    private Vector2 statekruch = Vector2.Zero;
    PackedScene pocisk;
    ShootButton przyciskDoStrzelania;
    SpecialButton przyciskDoSpeciala;
    public TextureProgressBar hpBar;
    public AnimationPlayer animacja;
    AnimatedSprite2D sprite;

    public override void _Ready()
    {
        pocisk = (PackedScene)ResourceLoader.Load("res://src/PlayerBullet.tscn");
        przyciskDoStrzelania = (ShootButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/ShootButton");
        przyciskDoSpeciala = (SpecialButton)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/SpecialButton");
        hpBar = (TextureProgressBar)GetTree().Root.GetNode("Game/Player/CanvasLayer/Control/HPBar");
        animacja = (AnimationPlayer)GetChild(2);
        sprite = (AnimatedSprite2D)GetTree().Root.GetNode("Game/Player/Statek/Statek_kadlub");
    }
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
    private void ReciveJoystick(Vector2 recivelJoystick)
    {
        joysticksygnal = recivelJoystick;
    }
    
    //shooting scripts
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
    
    public void ShootSpecial()
    {
        PlayerBullet nowyPocisk = (PlayerBullet)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        nowyPocisk.Scale = new Vector2(4, 4);
        nowyPocisk.bulletDmg = 100;
        nowyPocisk.speed = -150;
        GetTree().Root.GetNode("Game/Player").AddChild(nowyPocisk);
    }

    //on getting hit by bullet
    private void _on_body_entered(Node body) //connect body entered signal
    {
        if (body is EnemyBullet enemyBullet)
        {
            Hit(enemyBullet.bulletDmg);
            body.QueueFree(); //delete bullet after contact
        }
    }

    //getting damaged scripts
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

    public void ChangeProcessMode()
    {
        if (GetParent().ProcessMode == ProcessModeEnum.Disabled) GetParent().ProcessMode = ProcessModeEnum.Inherit;
        else GetParent().ProcessMode = ProcessModeEnum.Disabled;
    }

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
