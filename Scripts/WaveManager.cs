using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class WaveManager : Node
{
	[Export] public EnemySpawner Spawner;
	
	private List<Wave> waves = new List<Wave>();
	
	public override async void _Ready()
	{
		CreateWaves();
		await RunWaves();
	}
	
	public async Task RunWaves()
	{
		await ToSignal(GetTree().CreateTimer(5f), "timeout");
		
		for (int w = 0; w < waves.Count; w++)
		{
			Wave wave = waves[w];
			
			for (int i = 0; i < wave.enemyCount; i++)
			{
				Spawner.SpawnEnemy();
				
				await ToSignal(GetTree().CreateTimer(wave.spawnDelay), "timeout");
			}
			
			await ToSignal(GetTree().CreateTimer(2f), "timeout");
		}
	}
	
	private void CreateWaves()
	{
		waves.Add(new Wave(5, 1.0f));
		waves.Add(new Wave(10, 0.8f));
		waves.Add(new Wave(15, 0.6f));
	}
}
