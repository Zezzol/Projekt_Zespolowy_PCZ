using Godot;
using System;

public partial class Wave : Node2D
{
    PackedScene enemyScene;
	
    public override void _Ready()
	{
        enemyScene = (PackedScene)ResourceLoader.Load("res://src/Enemy.tscn");
    }

	public override void _Process(double delta)
	{
		if (this.GetChildCount() == 0) //sprawdz czy gracz pokonal wszystkich przeciwnikow w danej fali
		{
			GetParent().Call("StartWave");
		}
	}

	//enemy spawn
	public void AddEnemies(int enemyCount)
	{
		if (enemyCount > 6) enemyCount = 6;

		for (int i = 0; i < enemyCount; i++) //dodaj przeciwnikow
		{
			Enemy enemy = (Enemy)enemyScene.Instantiate();
			Random r = new Random();

			Vector2 enemyPosition = new Vector2(r.Next(100, 548), enemy.Position.Y); //losowa pozycja przeciwnikow
			enemy.Position = enemyPosition;

            AddChild(enemy);
        }
	}
}
