using Godot;
using System;

public partial class LevelMenu : Control
{
	private void _on_level_1_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game1.tscn";

		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn");
	}
	
	private void _on_level_2_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game2.tscn";

		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn");
	}
	
	private void _on_level_3_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game3.tscn";
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn"); 
	}
	
	private void _on_level_4_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game4.tscn";
    
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn"); 
	}
	
	private void _on_level_5_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game5.tscn";
    
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn"); 
	}
	
	private void _on_level_6_pressed()
	{
		GameData.NivelSeleccionado = "res://Escenas/Levels/Game6.tscn";
    
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn"); 
	}
	

	private void _on_quit_pressed()
	{
		//Cambia al menu principal
		GetTree().ChangeSceneToFile("res://Escenas/MainMenu.tscn");
	}
}
