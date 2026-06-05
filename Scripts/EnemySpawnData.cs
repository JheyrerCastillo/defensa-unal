using Godot;

[GlobalClass]
public partial class EnemySpawnData : Resource
{
	[Export] public EnemyType Type; //Tipo de enemigo a spawnear
	[Export] public int Count = 1; //Cantidad de enemigos a spawnear de un tipo
}