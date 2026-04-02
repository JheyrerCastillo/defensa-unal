using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	
	private MapManager mapManager;
	private TileMap tileMap;
	
	public override void _Ready()
	{
		var game = GetParent();
		
		mapManager = game.GetNode<MapManager>("MapManager");
		tileMap = game.GetNode<TileMap>("TileMap");
	}
	
	public void SpawnEnemy()
	{
		var enemy = EnemyScene.Instantiate<Enemy>();
		
		var path = mapManager.GetPath();
		
		enemy.SetPath(path, tileMap);
		
		AddChild(enemy);
	}
}
