using Godot;
using System;

public enum GameState
{
	Build,
	Combat
}

public partial class Game : Node2D
{
	public GameState currentState = GameState.Build;
	
	private WaveManager waveManager;
	
	private MoneyManager moneyManager;
	
	public override void _Ready()
	{
		waveManager = GetNode<WaveManager>("WaveManager");
		moneyManager = GetNode<MoneyManager>("MoneyManager");
	}
	
	public void StartWave()
	{
		if (currentState != GameState.Build) return;
		
		currentState = GameState.Combat;
		
		waveManager.StartWave();
	}
	
	public void EndWave()
	{
		currentState = GameState.Build;
	}
}
