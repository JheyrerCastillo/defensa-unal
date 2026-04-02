using Godot;
using System;

public partial class BuildManager : Node
{
	[Export] public PackedScene TowerScene;
	[Export] public PackedScene FastTowerScene;
	[Export] public PackedScene HeavyTowerScene;
	
	private TileMap tileMap;
	private MapManager mapManager;
	private Game game;
	
	public enum TowerType
	{
		Fast,
		Normal,
		Heavy
	}
	
	private TowerType selectedTower = TowerType.Normal;
	
	public override void _Ready()
	{
		var parent = GetParent();
		
		tileMap = parent.GetNode<TileMap>("TileMap");
		mapManager = parent.GetNode<MapManager>("MapManager");
		game = parent.GetNode<Game>(".");
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
				PackedScene sceneToSpawn = GetSelectedScene();
				
				if (sceneToSpawn == null) return;
				
				Node2D tower = sceneToSpawn.Instantiate<Node2D>();
				
				Vector2 worldPos = tileMap.MapToLocal(tilePos);
				tower.Position = worldPos;
				
				GetParent().AddChild(tower);
				
				mapManager.SetOcuppied(x,y);
			}
		}
	}
	
	private PackedScene GetSelectedScene()
	{
		switch (selectedTower)
		{
			case TowerType.Fast: return FastTowerScene;
			case TowerType.Normal: return TowerScene;
			case TowerType.Heavy: return HeavyTowerScene;
		}
		return null;
	}
	
	public void SetTower(TowerType type)
	{
		selectedTower = type;
	}
}
