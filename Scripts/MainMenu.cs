using Godot;

public partial class MainMenu : Control
{
	private void _on_play_pressed()
	{
		//Cambia a la escena del nivel
		GetTree().ChangeSceneToFile("res://Escenas/Main.tscn");
	}

	private void _on_quit_pressed()
	{
		//Cierra el programa
		GetTree().Quit();
	}
}
