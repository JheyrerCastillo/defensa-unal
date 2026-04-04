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
	
	private TowerType? selectedTower = null;
	
	private Dictionary<TowerType, PackedScene> towerScenes;
	
	public override void _Ready()
	{
		var parent = GetParent();
		
		tileMap = parent.GetNode<TileMap>("TileMap");
		mapManager = parent.GetNode<MapManager>("MapManager");
		game = parent.GetNode<Game>(".");
		moneyManager = parent.GetNode<MoneyManager>("MoneyManager");
		
		//Añade a un diccionario las torres dispoibles
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
		
		//Cuando el jugador hace click
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			//Obtine la ubicación del tile presionado
			Vector2 localPos = tileMap.ToLocal(mouseEvent.Position);
			Vector2I tilePos = tileMap.LocalToMap(localPos);
			
			int x = tilePos.X;
			int y = tilePos.Y;
			
			if (mapManager.CanBuild(x,y))
			{
				//Intancia la torre seleccionda
				if (!towerScenes.ContainsKey(selectedTower.Value)) return;
				PackedScene sceneToSpawn = towerScenes[selectedTower.Value];
				
				Tower towerInstance = sceneToSpawn.Instantiate<Tower>();
				int cost = towerInstance.Cost;
				
				//Gasta el dinero que gastó en la torre
				if (!moneyManager.SpendMoney(cost)) return;
				
				Vector2 worldPos = tileMap.MapToLocal(tilePos);
				towerInstance.Position = worldPos;
				
				//coloca la torre
				GetParent().AddChild(towerInstance);
				
				//Ocupa la casilla
				mapManager.SetOcuppied(x,y);
			}
		}
	}
	
	public void SetTower(TowerType type)
	{
		//La torre seleccionada
		selectedTower = type;
	}
}
