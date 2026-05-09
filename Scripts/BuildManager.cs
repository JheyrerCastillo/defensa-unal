using Godot;
using System.Collections.Generic;

public partial class BuildManager : Node
{
	[Export] public TowerData NormalTowerData;
	[Export] public TowerData FastTowerData;
	[Export] public TowerData HeavyTowerData;
	
	private TileMap tileMap; //Mapa en el que construir
	private MapManager mapManager; //Nodo que maneja mapas
	private Game game; //Nodo que maneja el juego
	private MoneyManager moneyManager; //Nodo que maneja el dinero
	
	private TowerType? selectedTower = null; //Tipo de torre seleccionada iniciada sin valor
	
	private Dictionary<TowerType, TowerData> towerDatas; //Diccionario de escenas de las torres
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		var parent = GetParent();
		
		tileMap = parent.GetNode<TileMap>("TileMap");
		mapManager = parent.GetNode<MapManager>("MapManager");
		game = parent.GetNode<Game>(".");
		moneyManager = parent.GetNode<MoneyManager>("MoneyManager");
		
		//Añade a un diccionario las torres disponibles
		towerDatas = new Dictionary<TowerType, TowerData>()
		{
			{TowerType.Fast, FastTowerData},
			{TowerType.Normal, NormalTowerData},
			{TowerType.Heavy, HeavyTowerData}
		};
	}
	
	public TowerData GetTowerData(TowerType type)
	{
		return towerDatas.GetValueOrDefault(type);
	}
	
	public void HandleInput(InputEvent @event)
	{
		//Verifica que no se esté presionando sobre la interfaz
		if (GetViewport().GuiGetHoveredControl() != null) return;
		
		//Cuando el jugador hace clic
		if (@event is InputEventMouseButton { Pressed: true } mouseEvent)
		{
			//Obtiene la ubicación del tile presionado
			Vector2 localPos = tileMap.ToLocal(mouseEvent.Position);
			Vector2I tilePos = tileMap.LocalToMap(localPos);
			
			//Guarda las coordenadas del tile presionado
			int x = tilePos.X;
			int y = tilePos.Y;
			
			//Si se puede construir...
			if (mapManager.CanBuild(x,y))
			{
				//Instancia la torre seleccionada y toma su costo
				if (!towerDatas.TryGetValue(selectedTower.Value, out var towerData)) return;
				Tower towerInstance = towerData.TowerScene.Instantiate<Tower>();
				towerInstance.Data = towerData;
				int cost = towerData.Cost;
				
				//Gasta el dinero que gastó en la torre
				if (!moneyManager.SpendMoney(cost)) return;
				
				//Guarda la posición seleccionada
				Vector2 worldPos = tileMap.MapToLocal(tilePos);
				towerInstance.Position = worldPos;
				
				//coloca la torre
				GetParent().AddChild(towerInstance);
				
				var evolutionPanel = GetTree().CurrentScene.GetNode<EvolutionPanel>("UI/EvolutionPanel");
				towerInstance.TowerSelected += evolutionPanel.ShowTower;
				
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
