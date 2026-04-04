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
		
		//Enemigos disposibles
		enemyScenes = new Dictionary<EnemyType, PackedScene>()
		{
			{EnemyType.Normal, EnemyScene}
		};
	}
	
	public void SpawnEnemy(EnemyType type)
	{
		if (!enemyScenes.ContainsKey(type)) return;
		PackedScene sceneToSpawn = enemyScenes[type];
		
		//Instancia enemigo
		var enemy = sceneToSpawn.Instantiate<Enemy>();
		
		var path = mapManager.GetPath();
		
		//Le dice el camino a seguir al enemigo
		enemy.SetPath(path, tileMap);
		
		//Lo coloca en el mapa
		AddChild(enemy);
	}
}
