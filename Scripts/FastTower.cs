public partial class FastTower : Tower 
{ 
	public override void _Ready()
	{
		FireRate = 4f; //Velocidad de la torre rapida
		base._Ready();
	}
}
