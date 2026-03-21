using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 300f;
	[Export] public int Damage = 1;
	
	private Enemy target;
	
	public void SetTarget(Enemy enemy)
	{
		target = enemy;
	}
	
	public override void _Process(double delta)
	{
		if (target == null || !IsInstanceValid(target))
		{
			QueueFree();
			return;
		}
		
		Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
		GlobalPosition += direction * Speed * (float)delta;
		
		Rotation = direction.Angle();
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
