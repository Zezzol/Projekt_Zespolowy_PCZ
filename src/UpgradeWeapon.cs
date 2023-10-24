using Godot;
using System;

public partial class UpgradeWeapon : Node2D
{
	int upgradeStat; //0 = upgrade HP, 1 = upgrade broni

	public override void _Ready()
	{
		Random rnd = new Random();
		upgradeStat = rnd.Next(2);
		
		if (upgradeStat == 0 )
		{
			Modulate = new Color("00ff00");
		}
	}

	public override void _Process(double delta)
	{
		Position += new Vector2(0, (float)(50 * delta));
	}

    public void _on_area_entered(Area2D area)
	{
        if (area is Statek statek)
        {
            if (upgradeStat == 0)
            {
				statek.maksHp += 50;
				statek.hp = statek.maksHp;
				statek.hpBar.MaxValue = statek.maksHp;
				statek.hpBar.Value = statek.hp;
            }
			else
			{
				if (statek.liczbaPociskow < 3)
				{
					statek.liczbaPociskow++;
					statek.Upgrade();
				}
			}

			this.QueueFree();
        }
	}
}
