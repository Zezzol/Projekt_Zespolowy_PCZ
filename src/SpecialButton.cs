using Godot;
using System;

public partial class SpecialButton : TouchScreenButton
{
    Node stateknode;
    public bool shootReady = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");
        this.Pressed += () =>
        {
            if (shootReady)
            {
                stateknode.Call("ShootSpecial");
            }
        }; //connect pressed signal
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
