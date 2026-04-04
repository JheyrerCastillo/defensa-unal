using Godot;
using System;
using System.Collections.Generic;

public partial class BuildManager : Node
{
	[Export] public PackedScene TowerScene;
	[Export] public PackedScene FastTowerScene;
	[Export] public PackedScene HeavyTowerScene;
	
	private TileMap tileMap;
	private MapManager mapManager;
	private Game game;
	private MoneyManager moneyManager;
	
	public enum TowerType {Fast, Normal, Heavy}
	
	private TowerType selectedTower = TowerType.Normal;
	
	private Dictionary<TowerType, PackedScene> towerScenes;
	
	public override void _Ready()
	{
		var parent = GetParent();
		
		tileMap = parent.GetNode<TileMap>("TileMap");
		mapManager = parent.GetNode<MapManager>("MapManager");
		game = parent.GetNode<Game>(".");
		moneyManager = parent.GetNode<MoneyManager>("MoneyManager");
		
		towerScenes = new Dictionary<TowerType, PackedScene>()
		{
			{TowerType.Fast, FastTowerScene},
			{TowerType.Normal, TowerScene},
			{TowerType.Heavy, HeavyTowerScene}
		};
	}
	
	public void HandleInput(InputEvent @event)
	{
		if (GetViewport().GuiGetHoveredControl() != null) return;
		
		if (game.currentState != GameState.Build) return;
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			Vector2 localPos = tileMap.ToLocal(mouseEvent.Position);
			Vector2I tilePos = tileMap.LocalToMap(localPos);
			
			int x = tilePos.X;
			int y = tilePos.Y;
			
			if (mapManager.CanBuild(x,y))
			{
				if (!towerScenes.ContainsKey(selectedTower)) return;
				PackedScene sceneToSpawn = towerScenes[selectedTower];
				
				Tower towerInstance = sceneToSpawn.Instantiate<Tower>();
				int cost = towerInstance.Cost;
				
				if (!moneyManager.SpendMoney(cost)) return;
				
				Vector2 worldPos = tileMap.MapToLocal(tilePos);
				towerInstance.Position = worldPos;
				
				GetParent().AddChild(towerInstance);
				
				mapManager.SetOcuppied(x,y);
			}
		}
	}
	
	public void SetTower(TowerType type)
	{
		selectedTower = type;
	}
}
