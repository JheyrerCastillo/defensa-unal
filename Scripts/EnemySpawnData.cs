using Godot;
using System;

public partial class EnemySpawnData
{
	public EnemyType type;
	public int count;
	
	public EnemySpawnData(EnemyType type, int count)
	{
		this.type = type;
		this.count = count;
	}
}
