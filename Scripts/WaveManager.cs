using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class WaveManager : Node
{
	[Export] public EnemySpawner Spawner; //Exporta en el inspector el spawner de enemigos
	
	private List<Wave> waves = new List<Wave>(); //Lista de oleadas
	
	public bool AllWavesFinished = false; //Verifica si las oleadas estaban finalizadas
	private int aliveEnemies = 0; //Enemigos vivos
	
	public override async void _Ready()
	{
		CreateWaves(); //Crea las oleadas
		await RunWaves(); //Inicia la secuencia de oleadas
	}
	
	public async Task RunWaves()
	{
		//Tiempo antes de que empiecen las oleadas
		await ToSignal(GetTree().CreateTimer(5f), "timeout");
		
		//Recorre todas las oleadas
		for (int w = 0; w < waves.Count; w++)
		{
			Wave wave = waves[w];
			
			//Recorre los tipos de enemigos dentro de la oleada
			foreach (var enemyData in wave.enemies)
			{
				//Spawnea la cantidad indicada de enmigos
				for (int i = 0; i < enemyData.count; i++)
				{
					//Spawnea al enemigo
					Spawner.SpawnEnemy(enemyData.type);
					
					//Espera un tiempo entre spawn
					await ToSignal(GetTree().CreateTimer(wave.spawnDelay), "timeout");
				}
			}
			
			//Espera un tiempo entre oleada
			await ToSignal(GetTree().CreateTimer(2f), "timeout");
			
			//Oleadas terminadas
			AllWavesFinished = true;
		}
	}
	
	public void RegisterEnemySpawn()
	{
		aliveEnemies++;
	}
	
	public void RegisterEnemyDeath()
	{
		aliveEnemies--;
		CheckWinConditions();
	}
	
	public void CheckWinConditions()
	{
		//Se verifiva que hayan terminado las oleadas
		if (!AllWavesFinished) return;
		
		//Si ya no hay más enemigos, ganas
		if (aliveEnemies <= 0) GetParent<Game>().Win();
	}
	
	//Define las oleadas del juego
	private void CreateWaves()
	{
		//Oleada 1
		waves.Add(new Wave(new List<EnemySpawnData>
		{
			new EnemySpawnData(EnemyType.Normal, 5)
		}, 1.0f));
		//Oleada 2
		waves.Add(new Wave(new List<EnemySpawnData>
		{
			new EnemySpawnData(EnemyType.Normal, 15)
		}, 0.75f));
		//Oleada 3
		waves.Add(new Wave(new List<EnemySpawnData>
		{
			new EnemySpawnData(EnemyType.Normal, 30)
		}, 0.5f));
	}
}
