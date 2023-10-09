using Godot;
using System;
using System.Reflection.Metadata;

public partial class ShootButton : TouchScreenButton
{
    Node stateknode;
    bool shooting = false;
    public bool shootReady = true;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        stateknode = GetTree().Root.GetNode("Player/Statek");
        this.Pressed += () => shooting = true; //connect pressed signal
        this.Released += () => shooting = false; //connect released signal
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (shootReady && shooting)
        {
            stateknode.Call("Shoot");
            shootReady = false;
        }
	}

}
