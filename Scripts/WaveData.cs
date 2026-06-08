using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WaveData : Resource
{
	[Export] public Array<EnemySpawnData> Enemies = new();

	[Export] public float SpawnDelay = 1f;

	[Export] public float DelayAfterWave = 5f;
	
	//Constructor de oleada, con los enemigos y el tiempo entre uno y otro
}
