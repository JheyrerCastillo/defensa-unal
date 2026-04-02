using Godot;
using System;

public partial class UIManager : Node
{
	[Export] private Control panelMenu;
	[Export] private Button toggleButton;
	
	private bool isPanelOpen = false;
	private float closedX;
	private float openX;
	
	private BuildManager buildManager;
	private Game game;
	
	public override void _Ready()
	{
		buildManager = GetNode<BuildManager>("../../Game/BuildManager");
		game = GetNode<Game>("../../Game");
		
		closedX = panelMenu.Position.X;
		openX = closedX - panelMenu.Size.X;
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
