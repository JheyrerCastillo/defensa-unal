using Godot;
using System;
using System.Collections.Generic;

public class Wave
{
	public List<EnemySpawnData> enemies;
	public float spawnDelay;
	
	public Wave(List<EnemySpawnData> enemies, float delay)
	{
		this.enemies = enemies;
		this.spawnDelay = delay;
	}
}
