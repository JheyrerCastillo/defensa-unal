using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	[Export] public int MaxHealth = 3;
	[Export] public int Reward = 10;
	
	private int currentHealth;
	
	private MoneyManager moneyManager;
	
	private List<Vector2> worldPath;
	private int index = 0;
	private float speed = 100f;
	
	private Game game;
	
	public int GetHealth()
	{
		return currentHealth;
	}
	
	public override void _Ready()
	{
		currentHealth = MaxHealth;
		game = GetTree().CurrentScene.GetNode<Game>("Game");
		moneyManager = GetTree().CurrentScene.GetNode<MoneyManager>("Game/MoneyManager");
	}
	
	public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		
		if (currentHealth <= 0)
		{
			Die();
		}
	}
	
	private void Die()
	{
		moneyManager.AddMoney(Reward);
		QueueFree();
	}
	
	public void SetPath(List<Vector2I> tilePath, TileMap tileMap)
	{
		worldPath = new List<Vector2>();
		
		foreach (var tile in tilePath)
		{
			worldPath.Add(tileMap.ToGlobal(tileMap.MapToLocal(tile)));
		}
		
		Position = worldPath[0];
		index = 1;
	}
	
	public override void _Process(double delta)
	{
		if (worldPath == null) return;
		
		if (index >= worldPath.Count)
		{
			game.GameOver();
			return;
		}
		
		Vector2 target = worldPath[index];
		
		Vector2 direction = (target - Position).Normalized();
		
		Position = Position.MoveToward(target, speed * (float)delta);
		
		if (Position.DistanceTo(target) < 2f)
		{
			index++;
		}
	}
	
	private void OnReachEnd()
	{
		
	}
}
