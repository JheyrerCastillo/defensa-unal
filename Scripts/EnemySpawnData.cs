public class EnemySpawnData
{
	public EnemyType type; //Tipo de enemigo a spawnear
	public int count; //Cantidad de enemigos a spawnear de un tipo
	
	//Constructor de datos del enemigo a spawnear
	public EnemySpawnData(EnemyType type, int count)
	{
		this.type = type;
		this.count = count;
	}
}
