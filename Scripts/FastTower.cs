using Godot;

public partial class FastTower : Tower 
{ 
	public override void _Ready()
	{
		GD.Print("Hola, soy la torre rapida");
		Firerate = 4f;
		base._Ready();
	}
}
