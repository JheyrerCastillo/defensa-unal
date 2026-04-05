using Godot;
using System;
using System.Collections.Generic;

public partial class BuildManager : Node
{
	[Export] public PackedScene TowerScene; //Exporta en el inspector la escena de la torre normal
	[Export] public PackedScene FastTowerScene; //Exporta en el inspector la escena de la torre rapìda
	[Export] public PackedScene HeavyTowerScene; //Exporta en el inspector la escena de la torre pesada
	
	private TileMap tileMap; //Mapa en el que construir
	private MapManager mapManager; //Nodo que maneja mapas
	private Game game; //Nodo que maneja el juego
	private MoneyManager moneyManager; //Nodo que maneja el dinero
	
	private TowerType? selectedTower = null; //Tipo de torre seleccionada iniciada sin valor
	
	private Dictionary<TowerType, PackedScene> towerScenes; //Diccionario de escenas de las torres
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
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
		//Verifica que no se esté presionando sobre la interfaz
		if (GetViewport().GuiGetHoveredControl() != null) return;
		
		//Cuando el jugador hace click
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			//Obtine la ubicación del tile presionado
			Vector2 localPos = tileMap.ToLocal(mouseEvent.Position);
			Vector2I tilePos = tileMap.LocalToMap(localPos);
			
			//Guarda las coordenadas del tile presionado
			int x = tilePos.X;
			int y = tilePos.Y;
			
			//Si se puede construir...
			if (mapManager.CanBuild(x,y))
			{
				//Intancia la torre seleccionda y toma su costo
				if (!towerScenes.ContainsKey(selectedTower.Value)) return;
				PackedScene sceneToSpawn = towerScenes[selectedTower.Value];
				Tower towerInstance = sceneToSpawn.Instantiate<Tower>();
				int cost = towerInstance.Cost;
				
				//Gasta el dinero que gastó en la torre
				if (!moneyManager.SpendMoney(cost)) return;
				
				//Guarda la posición seleccionada
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
