using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 5000f; //Velocidad de la bala
	[Export] public float Damage = 1; //Daño que hace la bala
	[Export] public PackedScene ColisionEffect;
	
	private Enemy target; //Enemigo objetivo de la bala
	private Vector2 targetDirection = Vector2.Zero; //Dirección del objetivo
	
	public void SetTarget(Enemy enemy)
	{
		//Toma el enemigo del argumento como objetivo de la bala
		target = enemy;
	}

	public void SetDirection(Vector2 direction)
	{
		//Toma el vector del argumento como la dirección del objetivo
		targetDirection = direction;
	}
	
	public override void _Process(double delta)
	{
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		/* Anterior condición target == null || !IsInstanceValid(target) || */
		if (GlobalPosition.X < 0  || GlobalPosition.X > 1153 || GlobalPosition.Y < 0 || GlobalPosition.Y > 640)
		{
			QueueFree();
			return;
		}
		
		/* Esto era para que la bala fuera un proyectil
		   lo que está ya obsoleto
		   Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized(); */
		
		//Se dirige a la posición que le indicó la torre
		GlobalPosition += targetDirection * Speed * (float)delta;
		sprite.Rotation += (float)delta * Speed/4;


		// Complemento de proyectiles: Rotation = direction.Angle();
	}
	
	private void OnBodyEntered(Node body)
	{
		//Impacta en el enemigo y la quita vida, se destruye bala
		if (body is Enemy enemy)
		{
			Node2D pow = (Node2D)ColisionEffect.Instantiate();
			pow.GlobalPosition = GlobalPosition;
			GetParent().AddChild(pow);
			enemy.TakeDamage(Damage);
			QueueFree();
			
		}
	}
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
}
