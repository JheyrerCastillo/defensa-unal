using Godot;
using System;

public partial class Game : Node2D
{
	private WaveManager waveManager;
	private MoneyManager moneyManager;
	private UIManager uiManager;
	
	public override void _Ready()
	{
		waveManager = GetNode<WaveManager>("WaveManager");
		moneyManager = GetNode<MoneyManager>("MoneyManager");
		uiManager = GetNode<UIManager>("../UI/UIManager");
	}
	
	public void GameOver()
	{
		//Pausa el juego y muestra pantalla de GameOver
		GetTree().Paused = true;
		uiManager.ShowGameOver();
	}
}
