using Godot;

public partial class EvolutionPanel : CanvasLayer
{
	private Label moneyLabel;
	private VBoxContainer container;
	private VBoxContainer buttonsContainer;
	private Tower currentTower;
	private MoneyManager moneyManager;
	private BuildManager buildManager;
	
	public void Inicializar(MoneyManager mm, BuildManager bm)
	{
		moneyManager = mm;
		buildManager = bm;

		moneyManager.MoneyChanged += OnMoneyChanged;
		OnMoneyChanged(moneyManager.GetMoney());
	}
	
	public override void _Ready()
	{
		moneyLabel = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/MoneyLabel");
		container = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer");
		buttonsContainer = GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/ButtonsContainer");
		Visible = false;
	}

	private void OnMoneyChanged(int money)
	{
		moneyLabel.Text = "Dinero: $" + money;
		if (Visible && currentTower != null)
		{
			UpdateButtons();
		}
	}

	public void ShowTower(Tower tower)
	{
		if (tower.Data.Evolutions.Count == 0) return;
		currentTower = tower;
		SetPanelPosition();
		Visible = true;
		
		foreach (Node child in buttonsContainer.GetChildren())
		{
			buttonsContainer.RemoveChild(child);
			child.QueueFree();
		}
		
		foreach (TowerData evolution in tower.Data.Evolutions)
		{
			Button button = new Button();
			button.SetMeta("Evolution", evolution);
			button.Text = evolution.Name + "\n$" + evolution.Cost;
			button.Disabled = moneyManager.GetMoney() < evolution.Cost;
			button.Pressed += () => UpgradeTower(evolution);
			buttonsContainer.AddChild(button);
		}
	}
	
	private void UpgradeTower(TowerData evolution)
	{
		if (!moneyManager.SpendMoney(evolution.Cost)) return;
		
		PackedScene scene = buildManager.GetTowerScene(evolution.TowerType);
		var upgradedTower = scene.Instantiate<Tower>();
		upgradedTower.Data = evolution;
		upgradedTower.GlobalPosition = currentTower.GlobalPosition;
		
		currentTower.GetParent().AddChild(upgradedTower);
		currentTower.QueueFree();
		
		Visible = false;
		currentTower = null;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Visible) return;

		if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
		{
			Vector2 mousePos = GetViewport().GetMousePosition();

			if (!GetNode<PanelContainer>("PanelContainer").GetGlobalRect().HasPoint(mousePos))
			{
				Visible = false;
			}
		}
	}

	private void SetPanelPosition()
	{
		var panel = GetNode<PanelContainer>("PanelContainer");
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		
		panel.ResetSize();
		Vector2 panelSize = panel.Size;
		Vector2 finalPos = mousePos + new Vector2(20,20);

		if (finalPos.X + panelSize.X > viewportSize.X) finalPos.X = mousePos.X - panelSize.X - 20;
		if (finalPos.Y + panelSize.Y > viewportSize.Y) finalPos.Y = mousePos.Y - panelSize.Y - 20;

		panel.GlobalPosition = finalPos;
	}

	private void UpdateButtons()
	{
		foreach (Button button in buttonsContainer.GetChildren())
		{
			TowerData evolution = (TowerData)button.GetMeta("Evolution");
			button.Disabled = moneyManager.GetMoney() < evolution.Cost;
		}
	}
}
