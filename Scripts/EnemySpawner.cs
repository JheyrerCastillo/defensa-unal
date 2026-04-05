using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene; //Exporta en el inspector la escena del enemigo
	
	private MapManager mapManager; //Nodo que maneja mapas
	private TileMap tileMap; //Mapa en el que spawnear enemigos
	
	private Dictionary<EnemyType, PackedScene> enemyScenes; //Diccionario de escenas de los enemigos
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		var game = GetParent();
		mapManager = game.GetNode<MapManager>("MapManager");
		tileMap = game.GetNode<TileMap>("TileMap");
		
		//Enemigos disponibles
		enemyScenes = new Dictionary<EnemyType, PackedScene>()
		{
			{EnemyType.Normal, EnemyScene}
		};
	}
	
	public void SpawnEnemy(EnemyType type)
	{
		//Instancia enemigo
		if (!enemyScenes.ContainsKey(type)) return;
		PackedScene sceneToSpawn = enemyScenes[type];
		var enemy = sceneToSpawn.Instantiate<Enemy>();
		
		//Camino por el que irá el enemigo
		var path = mapManager.GetPath();
		
		//Le dice el camino a seguir al enemigo
		enemy.SetPath(path, tileMap);
		
		//Lo coloca en el mapa
		AddChild(enemy);
	}
}
