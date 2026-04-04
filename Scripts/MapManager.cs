using Godot;
using System;
using System.Collections.Generic;

public partial class MapManager : Node
{
	private int[,] map;
	private int width;
	private int height;
	
	private TileMap tileMap;
	
	public override void _Ready()
	{
		tileMap = GetParent().GetNode<TileMap>("TileMap");
		InitializeMap();
	}
	
	private void InitializeMap()
	{
		//Obtiene el tileMap
		var usedCells = tileMap.GetUsedCells(0);
		
		//Limites
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
			//Matriz con las coordenadas de las casillas con su ID
			int sourceId = tileMap.GetCellSourceId(0, cell);
			
			if (sourceId == 1) map[cell.Y, cell.X] = 1; //ID del camino
			else if (sourceId == 2) map[cell.Y, cell.X] = 2; //ID del inicio del camino
			else if (sourceId == 3) map[cell.Y, cell.X] = 3; //ID del final del camino
			else if (sourceId == 4) map[cell.Y, cell.X] = 4; //ID de zona no construible
			else map[cell.Y, cell.X] = 0; //ID del suelo (construible)
		}
	}
	
	public bool CanBuild(int x, int y)
	{
		//Determina si en la casilla puede construirse una torre 
		if (x < 0 || y < 0 || x >= width || y >= height) return false;
		
		return map[y,x] == 0;
	}
	
	private Vector2I GetStart()
	{
		//Recorre la matriz y busca el inicio
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
		//Recorre la matriz y busca el final
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if(map[y,x] == 3) return new Vector2I(x,y);
			}
		}
		return Vector2I.Zero;
	}
	
	public List<Vector2I> GetPath()
	{
		//Obtiene la ubicacióm del inicio y del final
		Vector2I start = GetStart();
		Vector2I end = GetEnd();
		
		Queue<Vector2I> queue = new Queue<Vector2I>();
		Dictionary<Vector2I, Vector2I> cameFrom = new Dictionary<Vector2I, Vector2I>();
		
		queue.Enqueue(start);
		cameFrom[start] = start;
		
		//Direcciones posibles
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
		//Esta dentro de los limites del mapa
		return pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height;
	}
	
	private bool IsWalkable(Vector2I pos)
	{
		//La casilla es caminable
		int value = map[pos.Y, pos.X];
		return value == 1 || value == 2 || value == 3;
	}
	
	public void SetOcuppied(int x, int y)
	{
		//La casilla esta ocupada y no es construible
		map[y, x] = 4;
	}
}
