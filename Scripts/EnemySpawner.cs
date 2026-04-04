using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	
	private MapManager mapManager;
	private TileMap tileMap;
	
	private Dictionary<EnemyType, PackedScene> enemyScenes;
	
	public override void _Ready()
	{
		var game = GetParent();
		
		mapManager = game.GetNode<MapManager>("MapManager");
		tileMap = game.GetNode<TileMap>("TileMap");
		
		enemyScenes = new Dictionary<EnemyType, PackedScene>()
		{
			{EnemyType.Normal, EnemyScene}
		};
	}
	
	public void SpawnEnemy(EnemyType type)
	{
		if (!enemyScenes.ContainsKey(type)) return;
		PackedScene sceneToSpawn = enemyScenes[type];
		
		var enemy = sceneToSpawn.Instantiate<Enemy>();
		
		var path = mapManager.GetPath();
		
		enemy.SetPath(path, tileMap);
		
		AddChild(enemy);
	}
}
