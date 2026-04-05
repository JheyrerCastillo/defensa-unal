using Godot;
using System;

public partial class Main : Node2D
{
	private BuildManager buildManager; //Nodo que maneja la construcción de torres
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		buildManager = GetNode<BuildManager>("Game/BuildManager");
	}
	
	public override void _Input(InputEvent @event)
	{
		//Toma los clics y los transfiere para la construcción de torres
		buildManager.HandleInput(@event);
	}
}
