using System.Collections.Generic;

public class Wave(List<EnemySpawnData> enemies, float delay)
{
	public readonly List<EnemySpawnData> Enemies = enemies;
	public readonly float SpawnDelay = delay;

	//Constructor de oleada, con los enemigos y el tiempo entre uno y otro
}
