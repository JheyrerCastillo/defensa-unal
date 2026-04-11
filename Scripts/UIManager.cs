using Godot;

public partial class UiManager : Node
{
	[Export] private Control panelMenu; //Exporta em el inspector el panel de torres
	[Export] private Button toggleButton; //Exporta en el inspector el botón para abrir el panel
	[Export] private Control gameOverPanel; //Exporta en el inspector el panel de game over
	[Export] private Control winPanel; //Exporta en el inspector el panel de victoria¡
	
	private Button fastTowerButton; //Botón para la torre rapida
	private Button normalTowerButton; //Botón para la torre normal
	private Button heavyTowerButton; //Botón para la torre pesada
	
	private bool isPanelOpen; //Estado del panel (Abierto/Cerrado)
	private float closedX; //Posición del panel cerrado
	private float openX; //Posición del panel abierto
	
	private BuildManager buildManager; //Nodo que maneja la construcción de torres
	private MoneyManager moneyManager; //Nodo que maneja el dinero
	private Label moneyLabel; //Label que muestra el dinero
	private Game game; //Nodo que maneja el juego
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		buildManager = GetNode<BuildManager>("../../Game/BuildManager");
		moneyManager = GetNode<MoneyManager>("../../Game/MoneyManager");
		game = GetNode<Game>("../../Game");
		
		//Referencia de botones necesarios
		moneyLabel = GetNode<Label>("../Panel/MarginContainer/VBoxContainer/MoneyLabel");
		fastTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/FastTowerButton");
		normalTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/NormalTowerButton");
		heavyTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/HeavyTowerButton");
		
		//actualiza indicador de dinero con la disponible
		moneyManager.MoneyChanged += OnMoneyChanged;
		OnMoneyChanged(moneyManager.GetMoney());
		
		//Abre o cierra panel desplegable
		closedX = panelMenu.Position.X;
		openX = closedX - panelMenu.Size.X;
		
		//Pone el precio en los botones
		UpdateTowerButtons();
	}
	
	public void ShowWin()
	{
		//Muestra la pantalla de victoria
		winPanel.Visible = true;
	}

	private void MenuButtonPressed()
	{
		//Cambia a la escena del menú
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Escenas/MainMenu.tscn");
	}
	
	public void ShowGameOver()
	{
		//Muestra pantalla de GameOver
		gameOverPanel.Visible = true;
	}
	
	private void RestartButton()
	{
		//Reanuda el juego y reinicia la escena
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
	
	private int GetTowerCost(TowerType type)
	{
		//Toma la escena de la torre de BuildManager
		var scene = buildManager.GetTowerScene(type);
		if (scene == null) return 0;
		
		//Muestra el costo de las torres disponibles
		Tower tower = scene.Instantiate<Tower>();
		return tower.Cost;
	}
	
	private void UpdateTowerButtons()
	{
		//Guarda los costos de cada tipo de torre
		int fastCost = GetTowerCost(TowerType.Fast);
		int normalCost = GetTowerCost(TowerType.Normal);
		int heavyCost = GetTowerCost(TowerType.Heavy);
		
		//Actualiza el precio de los botones con su costo
		fastTowerButton.Text = "Fast\n$" + fastCost;
		normalTowerButton.Text = "Normal\n$" + normalCost;
		heavyTowerButton.Text = "Heavy\n$" + heavyCost;
	}
	
	private void UpdateButtonState()
	{
		//Guarda el dinero actual
		int money = moneyManager.GetMoney();
		
		//activa o desactiva boton de torre si hay dinero para comprarlo
		fastTowerButton.Disabled = money < GetTowerCost(TowerType.Fast);
		normalTowerButton.Disabled = money < GetTowerCost(TowerType.Normal);
		heavyTowerButton.Disabled = money < GetTowerCost(TowerType.Heavy);
	}
	
	private void OnMoneyChanged(int newAmount)
	{
		//Actualiza el indicador de dinero y el estado de los botones
		moneyLabel.Text = "Dinero: $" + newAmount;
		UpdateButtonState();
	}

	private void TogglePanel()
	{
		//Abre o cierra el panel lateral
		isPanelOpen = !isPanelOpen;
		
		Vector2 pos = panelMenu.Position;
		
		if (isPanelOpen)
		{
			pos.X = openX;
			toggleButton.Text = ">";
		} 
		else 
		{
			pos.X = closedX;
			toggleButton.Text = "<";
		}
		
		panelMenu.Position = pos;
		
		//Mueve el botón junto con el panel
		Vector2 buttonPos = toggleButton.Position;
		buttonPos.X = panelMenu.Position.X - toggleButton.Size.X;
		toggleButton.Position = buttonPos;
	}
	
	//Selecciona la torre rapida
	private void SelectedFastTower()
	{
		buildManager.SetTower(TowerType.Fast);
	}
	
	//Selecciona la torre normal
	private void SelectedNormalTower()
	{
		buildManager.SetTower(TowerType.Normal);
	}
	
	//Selecciona la torre pesada
	private void SelectedHeavyTower()
	{
		buildManager.SetTower(TowerType.Heavy);
	}
}
