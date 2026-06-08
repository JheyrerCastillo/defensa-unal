using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	// Exporta en el inspector los enemigos
	[Export] public PackedScene EnemyScene; 
	[Export] public PackedScene HeavyEnemyScene; 
	[Export] public PackedScene FastEnemyScene;
	[Export] public PackedScene MoabEnemyScene;
	
	protected MapManager mapManager; //Nodo que maneja mapas
	protected TileMap tileMap; //Mapa en el que spawnear enemigos
	protected WaveManager waveManager; //Nodo que maneja las oleadas
	protected Game game;
	protected MoneyManager moneyManager;
	
	protected Dictionary<EnemyType, PackedScene> enemyScenes; //Diccionario de escenas de los enemigos
	
	public int index = 0;
	public Vector2 position = Vector2.Zero;

	public void SetIndex(int Newindex)
	{
		index = Newindex;
	}

	public void SetPosition(Vector2 Newposition)
	{
		position = Newposition;
	}
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		game = GetParent<Game>();
		moneyManager = game.GetNode<MoneyManager>("MoneyManager");
		mapManager = game.GetNode<MapManager>("MapManager");
		tileMap = game.GetNode<TileMap>("TileMap");
		waveManager = game.GetNode<WaveManager>("WaveManager");
		
		//Enemigos disponibles
		enemyScenes = new Dictionary<EnemyType, PackedScene>()
		{
			{EnemyType.Normal, EnemyScene},
			{EnemyType.Fast, FastEnemyScene},
			{EnemyType.Heavy , HeavyEnemyScene},
			{EnemyType.Moab, MoabEnemyScene}
		};
	}
	
	public virtual void SpawnEnemy(EnemyType type ,bool Moab = false)
	{
		//Instancia un enemigo
		if (!enemyScenes.TryGetValue(type, out var sceneToSpawn)) return;
		var enemy = sceneToSpawn.Instantiate<Enemy>();
		enemy.Inicializar(game, moneyManager, waveManager);
		
		//Camino por el que irá el enemigo
		var path = mapManager.GetPath();
		
		//Le dice el camino a seguir al enemigo
		enemy.SetPath(path, tileMap);
		
		//Lo coloca en el mapa
		if (Moab == false)
		{
			AddChild(enemy);	
		}
		else
		{
			enemy.Position = position;
			enemy.SetIndex(index);
			AddChild(enemy);
		}
			
	}
}
