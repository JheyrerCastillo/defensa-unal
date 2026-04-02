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
	
	public override void _Ready()
	{
		waveManager = GetNode<WaveManager>("WaveManager");
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
