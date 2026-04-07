using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	[Export] public Texture2D SpriteUp;
	[Export] public Texture2D SpriteLeft;
	[Export] public Texture2D SpriteDown;
	[Export] public int MaxHealth = 3; //Vida total del enemigo
	[Export] public int Reward = 10; //Dinero obtenido al matar el enemigo

	private Sprite2D sprite; // Creo la variable sprite para después poder cambiarla
	private int currentHealth; //Vida maxima del enemigo
	private bool isDead;
	
	private Game game; //Nodo que maneja el juego
	private MoneyManager moneyManager; //Nodo que maneja el dinero
	private WaveManager waveManager; //Nodo que maneja las oleadas
	
	private List<Vector2> worldPath; //Lista de vectores del camino del enemigo
	private int index = 0; //Índice que indica hacia donde se mueve el enemigo
	protected float speed = 100f; //Velocidad del enemigo
	
	//Toma la vida actual del enemigo para otros scripts
	public float GetSpeed()
	{
		return speed;
	}
	
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Derecha");
		sprite.Texture = SpriteUp;
		game = GetTree().CurrentScene.GetNode<Game>("Game");
		moneyManager = GetTree().CurrentScene.GetNode<MoneyManager>("Game/MoneyManager");
		waveManager = GetTree().CurrentScene.GetNode<WaveManager>("Game/WaveManager");
		
		currentHealth = MaxHealth; //Inicia la vida actual del enemigo como la vida total
	}
	
	public void TakeDamage(int damage)
	{
		if (isDead) return;
		
		//recibe daño y pierde vida
		currentHealth -= damage;
		
		//Si llega a cero su vida, se muere
		if (currentHealth <= 0)
		{
			Die();
		}
	}
	
	private void Die()
	{
		isDead = true;
		//Se destruye y recompensa al jugador con una cantidad de dinero
		moneyManager.AddMoney(Reward);
		waveManager.RegisterEnemyDeath();
		QueueFree();
	}
	
	public void SetPath(List<Vector2I> tilePath, TileMap tileMap)
	{
		//Crea el camino del enemigo como una lista de vectores
		worldPath = new List<Vector2>();
		
		//Añade los tiles de camino del mapa al camino del enemigo
		foreach (var tile in tilePath)
		{
			worldPath.Add(tileMap.ToGlobal(tileMap.MapToLocal(tile)));
		}
		
		//Pone al enemigo en el inicio del camino y se moverá hacia el tile en el indice 1
		Position = worldPath[0];
		index = 1;
	}
	
	public override void _Process(double delta)
	{
		//Verifica que el camino del enemigo no esté vacío
		if (worldPath == null) return;
		
		//Si llega al final, el juego termina
		if (index >= worldPath.Count)
		{
			game.GameOver();
			return;
		}
		
		//Guarda el tile que corresponde al indice como el objetivo del enemigo
		Vector2 target = worldPath[index];
		
		//Guarda la dirección hacia la que irá el enemigo
		Vector2 direction = (target - Position).Normalized();
		double angulo = Math.Atan2(direction.Y, direction.X) *180 / Math.PI;
		// Ve a que dirrección se mueve el enmigo y cambia el sprite
		if (angulo < -60 && angulo > -150)
		{
			sprite.Texture = SpriteUp;
		}
		else if (angulo <= -150 || angulo >= 150)
		{
			sprite.Texture = SpriteLeft;
		}
		else
		{
			sprite.Texture = SpriteDown;
		}
		
		//Mueve al enemigo hacia el tile objetivo
		Position = Position.MoveToward(target, speed * (float)delta);
		
		//Hace que el enemigo vaya hacia el siguiente tile en la lista
		if (Position.DistanceTo(target) < 2f)
		{
			index++;
		}
	}
}
