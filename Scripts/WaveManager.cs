using Godot;
using System.Threading.Tasks;

public partial class WaveManager : Node
{
	[Export] public EnemySpawner Spawner; //Exporta en el inspector el spawner de enemigos
	
	[Export] public LevelData LevelData; //Lista de oleadas

	private bool allWavesFinished; //Verifica si las oleadas estaban finalizadas
	private int aliveEnemies; //Enemigos vivos
	
	public override async void _Ready()
	{
		await RunWaves(); //Inicia la secuencia de oleadas
	}

	private async Task RunWaves()
	{
		//Tiempo antes de que empiecen las oleadas
		await ToSignal(GetTree().CreateTimer(5f), "timeout");
		
		//Recorre todas las oleadas
		foreach (WaveData wave in LevelData.Waves)
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
}
