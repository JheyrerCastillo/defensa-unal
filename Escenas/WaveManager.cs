using Godot;
using System;
using System.Collections.Generic;

public partial class WaveManager : Node
{
	[Export] public EnemySpawner Spawner;
	
	private List<Wave> waves = new List<Wave>();
	private int currentWave = 0;
	
	public override void _Ready()
	{
		CreateWaves();
	}
	
	public async void StartWave()
	{
		if (currentWave >= waves.Count) return;
		
		Wave wave = waves[currentWave];
		
		for (int i = 0; i < wave.enemyCount; i++)
		{
			Spawner.SpawnEnemy();
			await ToSignal(GetTree().CreateTimer(wave.spawnDelay), "timeout");
		}
		
		currentWave++;
		
		GetParent<Main>().currentState = GameState.Build;
	}
	
	private void CreateWaves()
	{
		waves.Add(new Wave(5, 1.0f));
		waves.Add(new Wave(10, 0.8f));
		waves.Add(new Wave(15, 0.6f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
