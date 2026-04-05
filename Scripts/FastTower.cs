public partial class FastTower : Tower 
{ 
	public override void _Ready()
	{
		Firerate = 4f; //Velocidad de la torre rapida
		base._Ready();
	}
}
