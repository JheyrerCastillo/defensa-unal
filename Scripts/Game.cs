using Godot;
using System;

public partial class Game : Node2D
{
	private WaveManager waveManager;
	private MoneyManager moneyManager;
	
	public override void _Ready()
	{
		waveManager = GetNode<WaveManager>("WaveManager");
		moneyManager = GetNode<MoneyManager>("MoneyManager");
	}
}
