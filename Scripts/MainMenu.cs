using Godot;
using System;

public partial class MainMenu : Control
{
	public void _on_play_pressed()
	{
		//Cambia a la escena del nivel
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn");
	}
	
	public void _on_quit_pressed()
	{
		//Cierra el programa
		GetTree().Quit();
	}
}
