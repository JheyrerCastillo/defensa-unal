using Godot;
using System;
using System.Collections.Generic;

public partial class Tower : Node2D
{
	[Export] public TowerData Data;
	
	private readonly List<Enemy> enemiesInRange = new List<Enemy>(); //Lista de enemigos que están en rango

	[Signal]
	public delegate void TowerSelectedEventHandler(Tower tower);

	private bool canBeSelected;

	public override void _Ready()
	{
		var clickArea = GetNode<Area2D>("ClickArea");
		clickArea.InputEvent += OnInputEvent;

		//Añade o quita un enemigo que haya entrado en el rango de tiro
		var area = GetNode<Area2D>("Area2D");
		area.BodyEntered += OnEnemyEntered;
		area.BodyExited += OnEnemyExited;
		
		var timer = GetNode<Timer>("Timer");
		timer.WaitTime = 1f/Data.FireRate; // para que exista una cadencia de disparo
		timer.Timeout += OnShoot;
		
		GetTree().CreateTimer(0.2f).Timeout += () => canBeSelected = true;
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
		
		//Cola con prioridad con los enemigos más rápidos que esten en rango
		var pq = new PriorityQueue<Enemy,float>();
		foreach (var enemy in enemiesInRange)
		{
			pq.Enqueue(enemy, -enemy.GetSpeed());
		}
		
		//Retorna el enemigo más rápido
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

	public int GetCost()
	{
		return Data.Cost;
	}

	public float GetFireRate()
	{
		return Data.FireRate;
	}

	public string GetTowerName()
	{
		return Data.Name;
	}
	
	private void OnShoot()
	{
		//Identifica al objetivo
		var target = GetTarget();
		if (target == null) return;
		
		//Obtiene su dirección
		Vector2 direction = GetDirectionTo(target);
		
		//Instancia una bala
		var bullet = Data.BulletScene.Instantiate<Bullet>();
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

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton { Pressed: true } mouseEvent)
		{
			if (!canBeSelected) return;
			EmitSignal(SignalName.TowerSelected, this);
		}
	}
}
