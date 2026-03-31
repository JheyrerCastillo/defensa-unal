using Godot;
using System;

public class Wave
{
	public int enemyCount;
	public float spawnDelay;
	
	public Wave(int count, float delay)
	{
		enemyCount = count;
		spawnDelay = delay;
	}
}
