using Godot;
using System;

public partial class Joystick : TouchScreenButton
{
    [Export]
    private int granica = 48;
    [Export]
    private int ignore = 150;
    private Vector2 buttonSize;
    private Vector2 buttonPosition;
    Node stateknode;
    
    public override void _Ready()
    {
        buttonSize = new Vector2(TexturePressed.GetWidth(), TexturePressed.GetHeight());
        stateknode = GetTree().Root.GetNode("Game/Player/Statek");
        GD.Print(stateknode.Name);

    }
    public override void _Process(double delta)
    {
        if (!IsPressed())
        {
            Position = Vector2.Zero;
        }

        if (stateknode != null && stateknode.IsInsideTree())
        {
            stateknode.Call("ReciveJoystick", Position / granica);
        }
    }
    public override void _Input(InputEvent inputEvent)
    {
        if (!IsPressed())
        {
            Position = Vector2.Zero;
        }
   
        if(inputEvent is InputEventScreenDrag eventScreenDrag)
        {
            if((eventScreenDrag.Position - GlobalPosition).Length()>ignore)
            {
                return;
            }
            this.GlobalPosition = eventScreenDrag.Position - buttonSize / 2;
         
            if (Position.Length()  > granica)
            {
                Console.WriteLine(Position.Normalized() * granica);
                Position=(Position.Normalized()) * granica;
            }
        }
    }
   
}
