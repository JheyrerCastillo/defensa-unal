using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class WaveManager : Node
{
	[Export] public EnemySpawner Spawner; //Exporta en el inspector el spawner de enemigos
	
	private readonly List<Wave> waves = new List<Wave>(); //Lista de oleadas

	private bool allWavesFinished; //Verifica si las oleadas estaban finalizadas
	private int aliveEnemies; //Enemigos vivos
	
	public override async void _Ready()
	{
		CreateWaves(); //Crea las oleadas
		await RunWaves(); //Inicia la secuencia de oleadas
	}

	private async Task RunWaves()
	{
		//Tiempo antes de que empiecen las oleadas
		await ToSignal(GetTree().CreateTimer(5f), "timeout");
		
		//Recorre todas las oleadas
		foreach (Wave wave in waves)
		{
			//Recorre los tipos de enemigos dentro de la oleada
			foreach (var enemyData in wave.Enemies)
			{
				//Spawnea la cantidad indicada de enemigos
				for (int i = 0; i < enemyData.Count; i++)
				{
					//Spawnea al enemigo
					Spawner.SpawnEnemy(enemyData.Type);
					
					//Añade un enemigo a la lista de enemigos vivos
					RegisterEnemySpawn();
					
					//Espera un tiempo entre spawn
					await ToSignal(GetTree().CreateTimer(wave.SpawnDelay), "timeout");
				}
			}
			
			//Espera un tiempo entre oleada
			await ToSignal(GetTree().CreateTimer(5f), "timeout");
		}
		
		//Oleadas terminadas
		allWavesFinished = true;
		
		//Verifica si ganaste
		CheckWinConditions();
	}

	private void RegisterEnemySpawn()
	{
		aliveEnemies++;
	}
	
	public void RegisterEnemyDeath()
	{
		aliveEnemies--;
		CheckWinConditions();
	}

	private void CheckWinConditions()
	{
		//Se verifica que hayan terminado las oleadas
		if (!allWavesFinished) return;
		
		//Si ya no hay más enemigos, ganas
		if (aliveEnemies == 0) GetParent<Game>().Win();
	}
	
	//Define las oleadas del juego
	private void CreateWaves()
	{
		//Oleada 1
		waves.Add(new Wave([new EnemySpawnData(EnemyType.Normal, 5)], 1.0f));
		//Oleada 2
		waves.Add(new Wave([
			new EnemySpawnData(EnemyType.Normal, 15),
			new EnemySpawnData(EnemyType.Fast, 2)
		], 0.80f));
		//Oleada 3
		waves.Add(new Wave([
			new EnemySpawnData(EnemyType.Normal, 10),
			new EnemySpawnData(EnemyType.Fast, 2),
			new EnemySpawnData(EnemyType.Heavy, 1),
			new EnemySpawnData(EnemyType.Normal, 10),
			new EnemySpawnData(EnemyType.Fast, 2),
			new EnemySpawnData(EnemyType.Heavy, 1),
			new EnemySpawnData(EnemyType.Normal, 10)
		], 0.65f));
		//Oleada 4
		waves.Add(new Wave([
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Normal, 2),
			new EnemySpawnData(EnemyType.Normal, 3),
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Normal, 3),
			new EnemySpawnData(EnemyType.Heavy, 2)
		],0.50f));
		//Oleada 5
		waves.Add(new Wave([
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Heavy, 2),
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Heavy, 2),
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Heavy, 2),
			new EnemySpawnData(EnemyType.Fast, 5),
			new EnemySpawnData(EnemyType.Heavy, 2)
		],0.4f));
	}
}
