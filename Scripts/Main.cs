using Godot;
using System;
using System.Collections.Generic;

public enum GameState
{
	Build,
	Combat	
}

public partial class Main : Node2D
{
	[Export] public PackedScene TowerScene;
	[Export] public PackedScene FastTowerScene;
	[Export] public PackedScene HeavyTowerScene;
	
	public enum TowerType
	{
		Fast,
		Normal,
		Heavy
	}
	private TowerType selectedTower;
	
	private int[,] map;
	private TileMap tileMap;
	
	public GameState currentState = GameState.Build;
	
	private int height;
	private int width;
	
	public override async void _Ready()
	{
		tileMap = GetNode<TileMap>("TileMap");
		
		InitializeMap();
		
		PrintMap();
	}
	
	private void DrawMap()  //esta funcion es debug
	{
		for(int y=0; y < height; y++)
		{
			for(int x=0; x < width; x++)
			{
				int tileType = map[y,x];
				
				Vector2I pos = new Vector2I(x,y);
				
				if (tileType == 0)
				{
					tileMap.SetCell(0, pos, 0, new Vector2I(0,0));
				}
				else if (tileType == 1)
				{
					tileMap.SetCell(0, pos, 1, new Vector2I(0,0));
				}
			}
		}
	}
	
	private void InitializeMap()
	{
		tileMap = GetNode<TileMap>("TileMap");
		
		var usedCells = tileMap.GetUsedCells(0);
		
		int maxX = 0;
		int maxY = 0;
		
		foreach (var cell in usedCells)
		{
			if (cell.X > maxX) maxX = cell.X;
			if (cell.Y > maxY) maxY = cell.Y;
		}
		
		width = maxX + 1;
		height = maxY + 1;
		
		map = new int[height, width];
		
		foreach (var cell in usedCells)
		{
			int sourceId = tileMap.GetCellSourceId(0, cell);
			
			if (sourceId == 1) map[cell.Y, cell.X] = 1;
			else if (sourceId == 2) map[cell.Y, cell.X] = 2;
			else if (sourceId == 3) map[cell.Y, cell.X] = 3;
			else if (sourceId == 4) map[cell.Y, cell.X] = 4;
			else map[cell.Y, cell.X] = 0;
		}
	}
	
	private Vector2I GetStart()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if(map[y,x] == 2) return new Vector2I(x,y);
			}
		}
		return Vector2I.Zero;
	}
	
	private Vector2I GetEnd()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if(map[y,x] == 3) return new Vector2I(x,y);
			}
		}
		return Vector2I.Zero;
	}
	
	private void PrintMap()
	{
		for (int i = 0; i < height; i++)
		{
			string row = "";
			
			for (int j = 0; j < width; j++)
			{
				row += map[i,j] + "";
			}
		}
	}
	
	public bool CanBuild(int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height) return false;
		
		return map[y,x] == 0;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (GetViewport().GuiGetHoveredControl() != null) return;
		
		if (currentState != GameState.Build) return;
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			Vector2 localPos = tileMap.ToLocal(mouseEvent.Position);
			Vector2I tilePos = tileMap.LocalToMap(localPos);
			int x = tilePos.X;
			int y = tilePos.Y;
			
			if (CanBuild(x,y))
			{
				PackedScene sceneToSpawn = null;
				
				switch (selectedTower)
				{
					case TowerType.Fast:
						sceneToSpawn = FastTowerScene;
						break;
					
					case TowerType.Normal:
						sceneToSpawn = TowerScene;
						break;
					
					case TowerType.Heavy:
						sceneToSpawn = HeavyTowerScene;
						break;
				}
				
				if (sceneToSpawn != null)
				{
					Node2D tower = sceneToSpawn.Instantiate<Node2D>();
					
					Vector2 worldPos = tileMap.MapToLocal(tilePos);
					tower.Position = worldPos;
					
					AddChild(tower);
					
					map[y,x] = 4;
				}
			}
		}
	}
	
	public List<Vector2I> GetPath()
	{
		Vector2I start = GetStart();
		Vector2I end = GetEnd();
		
		Queue<Vector2I> queue = new Queue<Vector2I>();
		Dictionary<Vector2I, Vector2I> cameFrom = new Dictionary<Vector2I, Vector2I>();
		
		queue.Enqueue(start);
		cameFrom[start] = start;
		
		Vector2I[] directions = {
			new Vector2I(1,0),
			new Vector2I(-1,0),
			new Vector2I(0,1),
			new Vector2I(0,-1),
			new Vector2I(1,1),
			new Vector2I(-1,1),
			new Vector2I(1,-1),
			new Vector2I(-1,-1)
		};
		
		while (queue.Count > 0)
		{
			Vector2I current = queue.Dequeue();
			
			if (current == end) break;
			
			foreach (var dir in directions)
			{
				Vector2I next = current + dir;
				
				if (!IsInside(next)) continue;
				
				if (!IsWalkable(next)) continue;
				
				if (cameFrom.ContainsKey(next)) continue;
				
				if (dir.X != 0 && dir.Y != 0)
				{
					Vector2I side1 = new Vector2I(current.X + dir.X, current.Y);
					Vector2I side2 = new Vector2I(current.X, current.Y + dir.Y);
					
					if (!IsWalkable(side1) || !IsWalkable(side2)) continue;
				}
				
				queue.Enqueue(next);
				cameFrom[next] = current;
			}
		}
		
		List<Vector2I> path = new List<Vector2I>();
		
		if (!cameFrom.ContainsKey(end)) return path;
		
		Vector2I temp = end;
		
		while (temp != start)
		{
			path.Add(temp);
			temp = cameFrom[temp];
		}
		
		path.Add(start);
		path.Reverse();
		
		return path;
	}
	
	private bool IsInside(Vector2I pos)
	{
		return pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height;
	}
	
	private bool IsWalkable(Vector2I pos)
	{
		int value = map[pos.Y, pos.X];
		return value == 1 || value == 2 || value == 3;
	}
	
	public void _on_start_wave_button_pressed()
	{
		if (currentState != GameState.Build) return;
		
		currentState = GameState.Combat;
		
		GetNode<WaveManager>("WaveManager").StartWave();
	}
	
	public void _on_fast_tower_button_pressed()
	{
		selectedTower = TowerType.Fast;
	}

	public void _on_normal_tower_button_pressed()
	{
		selectedTower = TowerType.Normal;
	}

	public void _on_heavy_tower_button_pressed()
	{
		selectedTower = TowerType.Heavy;
	}
}
