using Godot;
using System.Collections.Generic;

public partial class MapManager : Node
{
	private int[,] map; //Arreglo de arreglos como mapa
	private int width; //Ancho del mapa
	private int height; //Alto del mapa
	
	private TileMap tileMap; //Mapa a manejar
	
	public override void _Ready()
	{
		//Referencia de nodos necesarios
		tileMap = GetParent().GetNode<TileMap>("TileMap");
		
		//Inicializa el mapa
		InitializeMap();
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
		//Obtiene la ubicación del inicio y del final
		Vector2I start = GetStart();
		Vector2I end = GetEnd();
		
		//Crea una nueva cola para el camino
		Queue<Vector2I> queue = new Queue<Vector2I>();
		//Crea un nuevo diccionario para saber de donde viene
		Dictionary<Vector2I, Vector2I> cameFrom = new Dictionary<Vector2I, Vector2I>();
		
		//Guarda el inicio en la cola de camino
		queue.Enqueue(start);
		//Determina que no hay nada antes del inicio
		cameFrom[start] = start;
		
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
		
		//Mientras el camino sea mayor a 0
		while (queue.Count > 0)
		{
			Vector2I current = queue.Dequeue();
			
			if (current == end) break; //Si llega al final, termina la busqueda
			
			//Para cada dirección...
			foreach (var dir in directions)
			{
				//Guarda el vector adyacente
				Vector2I next = current + dir;
				
				//Verifica si está en el límite
				if (!IsInside(next)) continue;
				
				//Verifica si se puede caminar por el vector adyacente
				if (!IsWalkable(next)) continue;
				
				//Evita nodos en los que ya se estuvo
				if (cameFrom.ContainsKey(next)) continue;
				
				//Atraviesa en diagonal solo si los tiles adyacentes al diagonal también son camino
				if (dir.X != 0 && dir.Y != 0)
				{
					Vector2I side1 = new Vector2I(current.X + dir.X, current.Y);
					Vector2I side2 = new Vector2I(current.X, current.Y + dir.Y);
					
					if (!IsWalkable(side1) || !IsWalkable(side2)) continue;
				}
				
				//Guarda el vector adyacente en la cola
				queue.Enqueue(next);
				//Guarda el vector actual como el vector del que proviene el siguiente
				cameFrom[next] = current;
			}
		}
		
		//Crea una lista de vectores que será el camino
		List<Vector2I> path = new List<Vector2I>();
		
		//Si no se encuentra camino, retorna la lista vacía
		if (!cameFrom.ContainsKey(end)) return path;
		
		//Toma el final como un vector temporal
		Vector2I temp = end;
		
		//Mientras el vector temporal sea diferente del inicial, se añade temp al camino y temp pasa a ser el uqe va antes
		while (temp != start)
		{
			path.Add(temp);
			temp = cameFrom[temp];
		}
		
		//Añade el inicio al final de la lista, quedando una lista con el camino de final a inicio
		path.Add(start);
		//Invierte el camino para que quede de inicio a final
		path.Reverse();
		
		//Retorna el camino completo
		return path;
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
