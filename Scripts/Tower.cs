using Godot;
using System;
using System.Collections.Generic;

public partial class Tower : Node2D
{
	[Export] public PackedScene BulletScene; //Munición que usara la torre
	[Export] public float Firerate = 2f; //Variable que determina cadencia de tiro
	[Export] public int Cost = 50;  //Costo de la torre
	
	private List<Enemy> enemiesInRange = new List<Enemy>(); //Lista de enemigos que estan en rango
	
	public override void _Ready()
	{
		//Añade o quita un enemgio que haya entrado en el rango de tiro
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
		
		//Si no hay enemigos en rango, no apunta a nada
		if (enemiesInRange.Count == 0) return null;
		
		var pq = new PriorityQueue<Enemy,int>();
		
		//cola con prioridad con los enemigos con más vida que esten en rango
		foreach (var enemy in enemiesInRange)
		{
			pq.Enqueue(enemy, -enemy.GetHealth());
		}
		
		return pq.Dequeue();
	}
	
	private Vector2 GetDirectionTo(Enemy target)
	{
		//La torre obtiene la dirección donde esta el enemigo
		return (target.GlobalPosition - GlobalPosition).Normalized();
	}
	
	private void OnShoot()
	{
		//Identific al objetivo
		var target = GetTarget();
		if (target == null) return;
		
		//Apunta al objetivo
		Vector2 direction = GetDirectionTo(target);
		
		//Instacia una bala
		var bullet = BulletScene.Instantiate<Bullet>();
		GetTree().CurrentScene.AddChild(bullet);
		
		//La bala va en dirección al objetivo
		bullet.GlobalPosition = GlobalPosition + 25 * direction;
		bullet.SetTarget(target);
		bullet.SetDirection(direction);
	}
	
	public override void _Process(double delta)
	{
		//Identifica un objetivo
		var target = GetTarget();
		if (target == null) return;
		
		//Lo apunta y obtiene su dirección
		Vector2 direction = GetDirectionTo(target);
		
		//Rota de acuerdo a donde esta apuntando
		Rotation = direction.Angle() + Mathf.Pi / 2;
	}
}
