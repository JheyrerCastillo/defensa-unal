using Godot;
using System;
using System.Collections.Generic;

public partial class Tower : Node2D
{
	[Export] public PackedScene BulletScene; //Munición que usara la torre
	[Export] public float Firerate = 1f; //Variable que determina cadencia de tiro
	[Export] public int Cost = 50;  //Costo de la torre
	
	private List<Enemy> enemiesInRange = new List<Enemy>(); //Lista de enemigos que están en rango
	
	public override void _Ready()
	{
		//Añade o quita un enemigo que haya entrado en el rango de tiro
		var area = GetNode<Area2D>("Area2D");
		area.BodyEntered += OnEnemyEntered;
		area.BodyExited += OnEnemyExited;
		
		var timer = GetNode<Timer>("Timer");
		timer.WaitTime = 1f/Firerate; // para que exista una cadencia de disparo
		timer.Timeout += OnShoot;
	}
	
	private void OnEnemyEntered(Node body)
	{
		//Si el cuerpo que entró en su rango es un enemigo, lo añade a la cola de enemigos en rango
		if (body is Enemy enemy) enemiesInRange.Add(enemy);
	}
	
	private void OnEnemyExited(Node body)
	{
		//Si el cuerpo que salió de su rango es un enemigo, lo elimina a la cola de enemigos en rango
		if (body is Enemy enemy) enemiesInRange.Remove(enemy);
	}
	
	private Enemy GetTarget()
	{
		//Elimina enemigos invalidos de la cola
		enemiesInRange.RemoveAll(e => !IsInstanceValid(e));
		
		//Si no hay enemigos en rango, no apunta a nada
		if (enemiesInRange.Count == 0) return null;
		
		//Cola con prioridad con los enemigos con más vida que esten en rango
		var pq = new PriorityQueue<Enemy,float>();
		foreach (var enemy in enemiesInRange)
		{
			pq.Enqueue(enemy, -enemy.GetSpeed());
		}
		
		//Retorna el enemigo con más vida
		return pq.Dequeue();
	}
	
	private Vector2 GetDirectionTo(Enemy target)
	{
		//La torre obtiene la dirección donde esta el enemigo
		return (target.GlobalPosition - GlobalPosition).Normalized();
	}

	protected virtual void PlayAnimationShoot()
	{
		
	}
	
	private void OnShoot()
	{
		//Identific al objetivo
		var target = GetTarget();
		if (target == null) return;
		
		//Obtiene su dirección
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
		
		//Obtiene su dirección
		Vector2 direction = GetDirectionTo(target);
		
		//Rota de acuerdo a la dirección
		double angulo = Math.Atan2(direction.X, direction.Y) * 180 / Math.PI - 90;
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		if (angulo <= -90 || angulo > 90)
		{
			sprite.FlipH = true;
		}
		else
		{
			sprite.FlipH = false;
		} 
		// Rotation = direction.Angle() + Mathf.Pi / 2;
	}
}
