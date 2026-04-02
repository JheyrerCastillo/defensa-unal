using Godot;
using System;

public partial class Main : Node2D
{	
	private MapManager mapManager;
	
	private Game game;
	
	private BuildManager buildManager;
	
	public override async void _Ready()
	{
		mapManager = GetNode<MapManager>("Game/MapManager");
		game = GetNode<Game>("Game");
		buildManager = GetNode<BuildManager>("Game/BuildManager");
	}
	
	public override void _Input(InputEvent @event)
	{
		buildManager.HandleInput(@event);
	}
}
