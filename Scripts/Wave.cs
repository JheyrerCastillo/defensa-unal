using Godot;
using System;
using System.Collections.Generic;

public class Wave
{
	public List<EnemySpawnData> enemies;
	public float spawnDelay;
	
	public Wave(List<EnemySpawnData> enemies, float delay)
	{
		//Constructor de oleada, con los enemigos y el tiempo entre uno y otro
		this.enemies = enemies;
		this.spawnDelay = delay;
	}
}
