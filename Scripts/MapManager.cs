using Godot;
using System;
using System.Collections.Generic;

public partial class MapManager : Node
{
	private int[,] map; //Arreglo de arreglos como mapa
	private int width; //Ancho del mapa
	private int height; //Alto del mapa
	
	private TileMap tileMap; //Mapa a manejar
	private List<List<Vector2I>> cachedPaths; //Caché de todos los caminos posibles
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		tileMap = GetParent().GetNode<TileMap>("TileMap");
		
		//Inicializa el mapa
		InitializeMap();
		
		//Cachea todos los caminos posibles
		Vector2I start = GetStart();
		Vector2I end = GetEnd();
		cachedPaths = FindAllPaths(start, end);
	}
	
	private void InitializeMap()
	{
		//Obtiene las celdas usadas del tilemap
		var usedCells = tileMap.GetUsedCells(0);
		
		//Limites inicializados en 0
		int maxX = 0;
		int maxY = 0;
		
		//Aumenta los límites del mapa si se usa una celda con mayor límite x o mayor límite y
		foreach (var cell in usedCells)
		{
			if (cell.X > maxX) maxX = cell.X;
			if (cell.Y > maxY) maxY = cell.Y;
		}
		
		//Incrementa el ancho y alto del mapa
		width = maxX + 1;
		height = maxY + 1;
		
		//Crea el mapa como una matriz con el tamaño basado en el ancho y alto
		map = new int[height, width];
		
		//Para cada celda entre las celdas usadas...
		foreach (var cell in usedCells)
		{
			//Toma el ID de la celda
			int sourceId = tileMap.GetCellSourceId(0, cell);
			
			//Cambia el valor de la celda en la matriz por el ID de la celda
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
		//Si no hay caminos cacheados, retorna lista vacía
		if (cachedPaths == null || cachedPaths.Count == 0) return new List<Vector2I>();
		
		//Retorna un camino aleatorio del caché
		Random random = new Random();
		int randomIndex = random.Next(cachedPaths.Count);
		return cachedPaths[randomIndex];
	}
	
	private List<List<Vector2I>> FindAllPaths(Vector2I start, Vector2I end)
	{
		List<List<Vector2I>> allPaths = new List<List<Vector2I>>();
		List<Vector2I> currentPath = new List<Vector2I>();
		HashSet<Vector2I> visited = new HashSet<Vector2I>();
		
		//Direcciones posibles
		Vector2I[] directions =
		[
			new Vector2I(1,0),
			new Vector2I(-1,0),
			new Vector2I(0,1),
			new Vector2I(0,-1),
			new Vector2I(1,1),
			new Vector2I(-1,1),
			new Vector2I(1,-1),
			new Vector2I(-1,-1)
		];
		
		FindAllPathsDFS(start, end, currentPath, visited, directions, allPaths);
		
		//Filtrar caminos con movimientos redundantes
		List<List<Vector2I>> filteredPaths = FilterRedundantPaths(allPaths);
		
		//Si todos los caminos fueron filtrados, retornar los originales
		if (filteredPaths.Count == 0 && allPaths.Count > 0)
		{
			return allPaths;
		}
		
		return filteredPaths;
	}
	
	private List<List<Vector2I>> FilterRedundantPaths(List<List<Vector2I>> paths)
	{
		List<List<Vector2I>> filteredPaths = new List<List<Vector2I>>();
		
		foreach (var path in paths)
		{
			if (!HasRedundantMovements(path))
			{
				filteredPaths.Add(path);
			}
		}
		
		return filteredPaths;
	}
	
	private bool HasRedundantMovements(List<Vector2I> path)
	{
		if (path.Count < 3) return false;
		
		for (int i = 2; i < path.Count; i++)
		{
			Vector2I prev = path[i - 2];
			Vector2I curr = path[i - 1];
			Vector2I next = path[i];
			
			//Detectar movimientos que se oponen directamente en el mismo eje
			Vector2I dir1 = curr - prev;
			Vector2I dir2 = next - curr;
			
			//Si el movimiento en X se opone (ej: derecha luego izquierda)
			if (dir1.X != 0 && dir2.X != 0 && Mathf.Sign(dir1.X) != Mathf.Sign(dir2.X))
			{
				return true;
			}
			
			//Si el movimiento en Y se opone (ej: arriba luego abajo)
			if (dir1.Y != 0 && dir2.Y != 0 && Mathf.Sign(dir1.Y) != Mathf.Sign(dir2.Y))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void FindAllPathsDFS(Vector2I current, Vector2I end, List<Vector2I> currentPath, 
		HashSet<Vector2I> visited, Vector2I[] directions, List<List<Vector2I>> allPaths)
	{
		//Añadir nodo actual al camino
		currentPath.Add(current);
		visited.Add(current);
		
		//Si llegamos al final, guardar el camino
		if (current == end)
		{
			allPaths.Add(new List<Vector2I>(currentPath));
		}
		else
		{
			//Explorar todas las direcciones
			foreach (var dir in directions)
			{
				Vector2I next = current + dir;
				
				//Verificar límites
				if (!IsInside(next)) continue;
				
				//Verificar si es caminable
				if (!IsWalkable(next)) continue;
				
				//Evitar nodos ya visitados
				if (visited.Contains(next)) continue;
				
				//Atraviesa en diagonal solo si los tiles adyacentes también son camino
				if (dir.X != 0 && dir.Y != 0)
				{
					Vector2I side1 = new Vector2I(current.X + dir.X, current.Y);
					Vector2I side2 = new Vector2I(current.X, current.Y + dir.Y);
					
					if (!IsWalkable(side1) || !IsWalkable(side2)) continue;
				}
				
				//Recursión
				FindAllPathsDFS(next, end, currentPath, visited, directions, allPaths);
			}
		}
		
		//Backtracking
		currentPath.RemoveAt(currentPath.Count - 1);
		visited.Remove(current);
	}
	
	private bool IsInside(Vector2I pos)
	{
		//Está dentro de los límites del mapa
		return pos is { X: >= 0, Y: >= 0 } && pos.X < width && pos.Y < height;
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
