using Godot;
public partial class Game : Node2D
{
	private WaveManager waveManager; //Nodo que maneja las oleadas
	private MoneyManager moneyManager; //Nodo que maneja el dinero
	private UIManager uiManager; //Nodo que maneja la interfaz de usuario
	
	public void Inicializar(UIManager uiRef)
	{
		uiManager = uiRef;
	}
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		waveManager = GetNode<WaveManager>("WaveManager");
		moneyManager = GetNode<MoneyManager>("MoneyManager");
	}
	
	public void GameOver()
	{
		//Pausa el juego y muestra pantalla de GameOver
		GetTree().Paused = true;
		uiManager.ShowGameOver();
	}
	
	public void Win()
	{
		//Pausa el juego y muestra la pantalla de victoria
		GetTree().Paused = true;
		uiManager.ShowWin();
	}
}
