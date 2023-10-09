using Godot;
using System;

public partial class Statek : CharacterBody2D
{
    private Vector2 joysticksygnal;
    private Vector2 statekruch = Vector2.Zero;
    public override void _Ready()
    {
       
    }
    public override void _PhysicsProcess(double delta)
    {
        statekruch.X = joysticksygnal.X;
        statekruch.Y = joysticksygnal.Y;
        MoveAndCollide(statekruch);
    }
    private void ReciveJoystick(Vector2 recivelJoystick)
    {
        joysticksygnal = recivelJoystick;
    }

}
