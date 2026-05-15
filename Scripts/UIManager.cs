using Godot;

public partial class UIManager : Node
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


	public void Inicializar(BuildManager bm, MoneyManager mm, Game gameRef)
	{
		buildManager = bm;
		moneyManager = mm;
		game = gameRef;

		//actualiza indicador de dinero
		moneyManager.MoneyChanged += OnMoneyChanged;
		OnMoneyChanged(moneyManager.GetMoney());

		//Pone el precio en los botones
		UpdateTowerButtons();
	}

	public override void _Ready()
	{
		//Referencia de botones necesarios
		moneyLabel = GetNode<Label>("../Panel/MarginContainer/VBoxContainer/MoneyLabel");
		fastTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/FastTowerButton");
		normalTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/NormalTowerButton");
		heavyTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/HeavyTowerButton");
		
		//Abre o cierra panel desplegable
		float screenWidth = GetViewport().GetVisibleRect().Size.X;
		closedX = screenWidth;
		openX = closedX - panelMenu.Size.X;
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
		var data = buildManager.GetTowerData(type);
		if (data == null) return 0;
		
		//Muestra el costo de las torres disponibles
		return data.Cost;
	}

	private string GetTowerName(TowerType type)
	{
		//Toma la escena de la torre de BuildManager
		var data = buildManager.GetTowerData(type);
		if (data == null) return "Nada";
		
		//Muestra el costo de las torres disponibles
		return data.Name;
	}
	
	private void UpdateTowerButtons()
	{
		//Guarda los nombres de cada tipo de torre
		string fastName = GetTowerName(TowerType.Fast);
		string normalName = GetTowerName(TowerType.Normal);
		string heavyName = GetTowerName(TowerType.Heavy);
		
		//Guarda los costos de cada tipo de torre
		int fastCost = GetTowerCost(TowerType.Fast);
		int normalCost = GetTowerCost(TowerType.Normal);
		int heavyCost = GetTowerCost(TowerType.Heavy);
		
		//Actualiza el precio de los botones con su costo
		fastTowerButton.Text = fastName + "\n$" + fastCost;
		normalTowerButton.Text = normalName + "\n$" + normalCost;
		heavyTowerButton.Text = heavyName + "\n$" + heavyCost;
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
