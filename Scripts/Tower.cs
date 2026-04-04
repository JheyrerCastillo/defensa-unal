using Godot;
using System;
using System.Collections.Generic;

public partial class Tower : Node2D
{
	[Export] public PackedScene BulletScene;
	[Export] public float Firerate = 2f;
	[Export] public int Cost = 50;
	
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
		if (body is Enemy enemy) enemiesInRange.Add(enemy);
	}
	
	private void OnEnemyExited(Node body)
	{
		if (body is Enemy enemy) enemiesInRange.Remove(enemy);
	}
	
	private Enemy GetTarget()
	{
		enemiesInRange.RemoveAll(e => !IsInstanceValid(e));
		
		if (enemiesInRange.Count == 0) return null;
		
		var pq = new PriorityQueue<Enemy,int>();
		
		foreach (var enemy in enemiesInRange)
		{
			pq.Enqueue(enemy, -enemy.GetHealth());
		}
		
		return pq.Dequeue();
	}
	
	private Vector2 GetDirectionTo(Enemy target)
	{
		return (target.GlobalPosition - GlobalPosition).Normalized();
	}
	
	private void OnShoot()
	{
		var target = GetTarget();
		if (target == null) return;
		
		Vector2 direction = GetDirectionTo(target);
		
		var bullet = BulletScene.Instantiate<Bullet>();
		GetTree().CurrentScene.AddChild(bullet);
		
		bullet.GlobalPosition = GlobalPosition + 25 * direction;
		bullet.SetTarget(target);
		bullet.SetDirection(direction);
	}
	
	public override void _Process(double delta)
	{
		var target = GetTarget();
		if (target == null) return;
		
		Vector2 direction = GetDirectionTo(target);
		
		Rotation = direction.Angle() + Mathf.Pi / 2;
	}
}
