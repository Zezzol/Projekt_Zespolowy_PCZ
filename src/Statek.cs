using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Statek : CharacterBody2D
{
    private Vector2 joysticksygnal;
    private Vector2 statekruch = Vector2.Zero;
    PackedScene pocisk;
    ShootButton przyciskDoStrzelania;
    SpecialButton przyciskDoSpeciala;
    public override void _Ready()
    {
        pocisk = (PackedScene)ResourceLoader.Load("res://src/Pocisk.tscn");
        przyciskDoStrzelania = (ShootButton)GetTree().Root.GetNode("Player/CanvasLayer/Control/ShootButton");
        przyciskDoSpeciala = (SpecialButton)GetTree().Root.GetNode("Player/CanvasLayer/Control/SpecialButton");
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

    public async void Shoot() //Wygeneruj pocisk, ustaw tor lotu i wystrzel
    {
        Pocisk nowyPocisk = (Pocisk)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        GetTree().Root.GetNode("Player").AddChild(nowyPocisk);

        await ToSignal(GetTree().CreateTimer(0.2), "timeout"); //delay beetwen shots
        przyciskDoStrzelania.shootReady = true;
    }
    
    public void ShootSpecial()
    {
        Pocisk nowyPocisk = (Pocisk)pocisk.Instantiate();
        nowyPocisk.Position = new Vector2(this.Position.X, this.Position.Y - 17);
        nowyPocisk.Scale = new Vector2(4, 4);
        GetTree().Root.GetNode("Player").AddChild(nowyPocisk);
        przyciskDoSpeciala.shootReady = false;
    }
}
