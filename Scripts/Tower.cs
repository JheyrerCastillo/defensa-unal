using Godot;
using System;
using System.Collections.Generic;

public partial class Tower : Node2D
{
	[Export] public PackedScene BulletScene;
	[Export] public float Firerate = 2f;
	
	private List<Enemy> enemiesInRange = new List<Enemy>();
	
	public override void _Ready()
	{
		var area = GetNode<Area2D>("Area2D");
		
		area.BodyEntered += OnEnemyEntered;
		area.BodyExited += OnEnemyExited;
		var timer = GetNode<Timer>("Timer");
		timer.WaitTime = 1f/Firerate; // para que exista una cadencia de disparo
		timer.Timeout += OnShoot;
	}
	
	private void OnEnemyEntered(Node body)
	{
		if (body is Enemy enemy)
		{
			enemiesInRange.Add(enemy);
		}
	}
	
	private void OnEnemyExited(Node body)
	{
		if (body is Enemy enemy)
		{
			enemiesInRange.Remove(enemy);
		}
	}
	
	private void OnShoot()
	{
		if(enemiesInRange.Count == 0) return;
		
		var target = enemiesInRange[0];
		
		var bullet = BulletScene.Instantiate<Bullet>();
		GetTree().CurrentScene.AddChild(bullet);
		Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
		
		bullet.GlobalPosition = GlobalPosition + 25 * (target.GlobalPosition - GlobalPosition).Normalized();
		bullet.SetTarget(target);
		bullet.SetDirection(direction);
	}
	
	public override void _Process(double delta)
	{
		if (enemiesInRange.Count == 0) return;
		
		Enemy target = enemiesInRange[0];
		
		Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
		
		Rotation = direction.Angle() + Mathf.Pi / 2;
	}
}
