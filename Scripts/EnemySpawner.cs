using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	
	private int enemiesToSpawn = 5;
	private int spawned = 0;
	
	private Main main;
	
	public override void _Ready()
	{
		main = GetParent<Main>();
	}
	
	public void _on_timer_timeout()
	{
		if (spawned >= enemiesToSpawn) return;
		
		SpawnEnemy();
		spawned++;
	}
	
	public void SpawnEnemy()
	{
		var enemy = EnemyScene.Instantiate<Enemy>();
		
		var path = main.GetPath();
		
		enemy.SetPath(path, main.GetNode<TileMap>("TileMap"));
		
		AddChild(enemy);
	}
}
