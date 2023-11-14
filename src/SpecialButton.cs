using Godot;
using System;

public partial class SpecialButton : TouchScreenButton
{
    Node stateknode;
    TextureProgressBar progressbar;
    public int enemyCountRequired = 10;
    public int enemyCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");
        progressbar = (TextureProgressBar)GetChild(0);

        this.Pressed += () =>
        {
            if (enemyCount == enemyCountRequired)
            {
                stateknode.Call("ShootSpecial");
                enemyCount = 0;
                progressbar.Value = 0;
            }
        }; //connect pressed signal
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
