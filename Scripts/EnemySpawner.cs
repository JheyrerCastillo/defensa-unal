using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	// Exporta en el inspector los enemigos
	[Export] public PackedScene EnemyScene; 
	[Export] public PackedScene HeavyEnemyScene; 
	[Export] public PackedScene FastEnemyScene; 
	
	private MapManager mapManager; //Nodo que maneja mapas
	private TileMap tileMap; //Mapa en el que spawnear enemigos
	private WaveManager waveManager; //Nodo que maneja las oleadas
	
	private Dictionary<EnemyType, PackedScene> enemyScenes; //Diccionario de escenas de los enemigos
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		var game = GetParent();
		mapManager = game.GetNode<MapManager>("MapManager");
		tileMap = game.GetNode<TileMap>("TileMap");
		waveManager = game.GetNode<WaveManager>("WaveManager");
		
		//Enemigos disponibles
		enemyScenes = new Dictionary<EnemyType, PackedScene>()
		{
			{EnemyType.Normal, EnemyScene},
			{EnemyType.Fast, FastEnemyScene},
			{EnemyType.Heavy , HeavyEnemyScene}
		};
	}
	
	public void SpawnEnemy(EnemyType type)
	{
		//Instancia un enemigo
		if (!enemyScenes.TryGetValue(type, out var sceneToSpawn)) return;
		var enemy = sceneToSpawn.Instantiate<Enemy>();
		
		//Camino por el que irá el enemigo
		var path = mapManager.GetPath();
		
		//Le dice el camino a seguir al enemigo
		enemy.SetPath(path, tileMap);
		
		//Lo coloca en el mapa
		AddChild(enemy);
	}
}
