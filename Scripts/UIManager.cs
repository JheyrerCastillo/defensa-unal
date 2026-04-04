using Godot;
using System;

public partial class UIManager : Node
{
	[Export] private Control panelMenu;
	[Export] private Button toggleButton;
	
	[Export] public PackedScene TowerScene;
	[Export] public PackedScene FastTowerScene;
	[Export] public PackedScene HeavyTowerScene;
	
	private Button fastTowerButton;
	private Button normalTowerButton;
	private Button heavyTowerButton;
	
	private bool isPanelOpen = false;
	private float closedX;
	private float openX;
	
	private BuildManager buildManager;
	private MoneyManager moneyManager;
	private Label moneyLabel;
	private Game game;
	
	public override void _Ready()
	{
		buildManager = GetNode<BuildManager>("../../Game/BuildManager");
		moneyManager = GetNode<MoneyManager>("../../Game/MoneyManager");
		game = GetNode<Game>("../../Game");
		
		moneyLabel = GetNode<Label>("../Panel/MarginContainer/VBoxContainer/MoneyLabel");
		fastTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/FastTowerButton");
		normalTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/NormalTowerButton");
		heavyTowerButton = GetNode<Button>("../Panel/MarginContainer/VBoxContainer/HeavyTowerButton");
		
		moneyManager.MoneyChanged += OnMoneyChanged;
		
		OnMoneyChanged(moneyManager.GetMoney());
		
		closedX = panelMenu.Position.X;
		openX = closedX - panelMenu.Size.X;
		
		UpdateTowerButtons();
	}
	
	private int GetTowerCost(PackedScene scene)
	{
		Tower tower = scene.Instantiate<Tower>();
		return tower.Cost;
	}
	
	private void UpdateTowerButtons()
	{
		int fastCost = GetTowerCost(FastTowerScene);
		int normalCost = GetTowerCost(TowerScene);
		int heavyCost = GetTowerCost(HeavyTowerScene);
		
		fastTowerButton.Text = "Fast\n$" + fastCost;
		normalTowerButton.Text = "Normal\n$" + normalCost;
		heavyTowerButton.Text = "Heavy\n$" + heavyCost;
	}
	
	private void UpdateButtonState()
	{
		int money = moneyManager.GetMoney();
		
		fastTowerButton.Disabled = money < GetTowerCost(FastTowerScene);
		normalTowerButton.Disabled = money < GetTowerCost(TowerScene);
		heavyTowerButton.Disabled = money < GetTowerCost(HeavyTowerScene);
	}
	
	private void OnMoneyChanged(int newAmount)
	{
		moneyLabel.Text = "Dinero: $" + newAmount;
		UpdateButtonState();
	}
	
	public void TogglePanel()
	{
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
		
		Vector2 buttonPos = toggleButton.Position;
		buttonPos.X = panelMenu.Position.X - toggleButton.Size.X;
		toggleButton.Position = buttonPos;
	}
	
	public void OnStartWavePressed()
	{
		game.StartWave();
		
		if (isPanelOpen)
		{
			isPanelOpen = false;
			
			Vector2 pos = panelMenu.Position;
			pos.X = closedX;
			panelMenu.Position = pos;
			
			toggleButton.Text = "<";
			
			Vector2 buttonPos = toggleButton.Position;
			buttonPos.X = panelMenu.Position.X - toggleButton.Size.X;
			toggleButton.Position = buttonPos;
		}
	}
	
	public void SelectedFastTower()
	{
		buildManager.SetTower(BuildManager.TowerType.Fast);
	}
	
	public void SelectedNormalTower()
	{
		buildManager.SetTower(BuildManager.TowerType.Normal);
	}
	
	public void SelectedHeavyTower()
	{
		buildManager.SetTower(BuildManager.TowerType.Heavy);
	}
}
