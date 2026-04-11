public partial class HeavyTower : Tower
{
	public override void _Ready()
	{
		FireRate = 1f; //Velocidad de disparo de la torre pesada
		base._Ready();
	}
}
