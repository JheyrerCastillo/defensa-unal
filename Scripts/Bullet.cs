using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 3000f;
	[Export] public int Damage = 1;
	
	private Enemy target;
	private Vector2 Targetdirection = Vector2.Zero;
	
	public void SetTarget(Enemy enemy)
	{
		target = enemy;
	}

	public void SetDirection(Vector2 Direction)
	{
		Targetdirection = Direction;
	}
	
	public override void _Process(double delta)
	{
		/* Anterior condición target == null || !IsInstanceValid(target) || */
		if (GlobalPosition.X < 0  || GlobalPosition.X > 1153 || GlobalPosition.Y < 0 || GlobalPosition.Y > 640)
		{
			QueueFree();
			return;
		}
		
		/* Esto era para que la bala fuera un proyectil
		   lo que esta ya obsoleto
		   Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized(); */
		
		GlobalPosition += Targetdirection * Speed * (float)delta;
		
		// Complemento de protectiles: Rotation = direction.Angle();
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
			QueueFree();
		}
	}
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		
	}
}
