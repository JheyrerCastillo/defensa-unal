using Godot;
using System;

public partial class Main : Node2D
{
	private BuildManager buildManager;
	
	public override void _Ready()
	{
		//Instacia el buildManager
		buildManager = GetNode<BuildManager>("Game/BuildManager");
	}
	
	public override void _Input(InputEvent @event)
	{
		buildManager.HandleInput(@event);
	}
}
