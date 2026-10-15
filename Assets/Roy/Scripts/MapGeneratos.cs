// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;
using System.Collections.Generic;

public class MapGeneratos : MonoBehaviour // Corregir la ortografía del nombre; Usar inglés
{
    [Header("Generacion")]
    [SerializeField] private int _targetRoomCount = 10;
    [SerializeField] private int _minAcceptableRooms = 6;
    [SerializeField] private int _maxAttempts = 300;
    [SerializeField] private int _maxRegenerations = 20;
    [SerializeField] private Vector2Int _gridHalfSize = new Vector2Int(4, 4);
    [SerializeField] [Range(0f, 1f)] private float _jumpChance = 0.15f; // Aclarar el significado o las unidades con Tooltip

    [Header("Salas")]
    [SerializeField] private GameObject normalRoomPrefab; // Usar _camelCase
    [SerializeField] private GameObject startRoomPrefab; // Usar _camelCase
    [SerializeField] private GameObject bossRoomPrefab; // Usar _camelCase
    [SerializeField] private GameObject treasureRoomPrefab; // Usar _camelCase
    [SerializeField] private GameObject shopRoomPrefab; // Usar _camelCase
    [SerializeField] private Vector2 roomWorldSize = new Vector2(20f, 11f); // Usar _camelCase

    private Dictionary<Vector2Int, RoomNode> rooms = new Dictionary<Vector2Int, RoomNode>();

    private static readonly Vector2Int[] Directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right}; // Usar _camelCase; Respetar el orden de acceso y static

    void Start()
    {
        GenerateMapWithRetries();
        AssignSpecialRooms();
        ValidateMap();
        PrintMapToConsole();
        InstantiateRooms();
    }

    private void GenerateMapWithRetries()
    {
        int regenerations = 0;
        
        do
        {
          regenerations++;
          GenerateLayout();  
        } 
        while (rooms.Count < _minAcceptableRooms && regenerations < _maxRegenerations);

        if (rooms.Count < _minAcceptableRooms)
        {
            Debug.LogError($"No se logro generar un mapa valido despues de {_maxRegenerations} intentos, ultimo resultado: {rooms.Count} salas");
        }
    }

    private void GenerateLayout()
    {
        rooms.Clear();

        Vector2Int current = Vector2Int.zero;
        var start = new RoomNode(current) {type = RoomType.Start}; // Usar el tipo explícito en lugar de var
        rooms.Add(current, start);
        
        int attempts = 0;

        while (rooms.Count < _targetRoomCount && attempts < _maxAttempts)
        {
            attempts++;

            Vector2Int direction = Directions[Random.Range(0, Directions.Length)];
            Vector2Int next = current + direction;

            if (!IsInsideGrid(next)) // Agregar llaves al if
            continue;

            if (rooms.ContainsKey(next))
            {
                current = next;
            }
            else
            {
                if (CountOccupiedNeighbours(next) > 1) // Agregar llaves al if
                continue;

                var newRoom = new RoomNode(next); // Usar el tipo explícito en lugar de var
                rooms.Add(next, newRoom);

                ConnectRooms(current, next);

                current = next;
            }

            if (Random.value < _jumpChance)
            {
                var keys = new List<Vector2Int>(rooms.Keys); // Usar el tipo explícito en lugar de var
                current = keys[Random.Range(0, keys.Count)];
            }
        }
    }

    private bool IsInsideGrid(Vector2Int pos) // Sin abreviaciones
    {
        return Mathf.Abs(pos.x) <= _gridHalfSize.x && Mathf.Abs(pos.y) <= _gridHalfSize.y;
    }

    private int CountOccupiedNeighbours(Vector2Int pos) // Sin abreviaciones
    {
        int count = 0;
        foreach (var dir in Directions) // Usar el tipo explícito en lugar de var
        {
            if (rooms.ContainsKey(pos + dir)) // Agregar llaves al if
            count++;
        }
        return count;
    }

    private void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        rooms[a].connections.Add(b-a); // Agregar espacios alrededor del operador
        rooms[b].connections.Add(a-b); // Agregar espacios alrededor del operador
    }

    private void AssignSpecialRooms()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        List<Vector2Int> leaves = new List<Vector2Int>();
        foreach (var kv in rooms) // Usar el tipo explícito en lugar de var
        {
            if (kv.Value.type == RoomType.Normal && kv.Value.connections.Count == 1) // Agregar llaves al if
            leaves.Add(kv.Key);
        }

        if (leaves.Count == 0)
        {
            Debug.LogWarning("No hay salas hoja disponibles y el mapa se quedada sin salas especiales");
            return;
        }

        leaves.Sort((a, b) => distances[b].CompareTo(distances[a]));

        Vector2Int bossPos = leaves[0]; // Sin abreviaciones
        rooms[bossPos].type = RoomType.Boss;
        leaves.RemoveAt(0);

        if (leaves.Count > 0)
        {
            int index = Random.Range(0, leaves.Count);
            rooms[leaves[index]].type = RoomType.Treasure;
            leaves.RemoveAt(index);
        }

        if (leaves.Count > 0)
        {
            int index = Random.Range(0, leaves.Count);
            rooms[leaves[index]].type = RoomType.Shop;
        }
    }

    private Dictionary<Vector2Int, int> ComputeDistanceFromStart()
    {
        var distances = new Dictionary<Vector2Int, int>(); // Usar el tipo explícito en lugar de var
        var queue = new Queue<Vector2Int>(); // Usar el tipo explícito en lugar de var

        Vector2Int startPos = Vector2Int.zero; // Sin abreviaciones
        distances[startPos] = 0;
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var direction in rooms[current].connections) // Usar el tipo explícito en lugar de var
            {
                Vector2Int neighbor = current + direction;

                if (!distances.ContainsKey(neighbor))
                {
                    distances[neighbor] = distances[current] + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return distances;
    }

    private void ValidateMap() // Revisar: una sola responsabilidad
    {
        bool isValid = true;

        foreach (var kv in rooms) // Usar el tipo explícito en lugar de var
        {
            Vector2Int pos = kv.Key; // Sin abreviaciones
            RoomNode node = kv.Value;

            foreach (var direction in node.connections) // Usar el tipo explícito en lugar de var
            {
                Vector2Int neighborPos = pos + direction; // Sin abreviaciones

                if (!rooms.ContainsKey(neighborPos))
                {
                    Debug.LogError($"[ValidateMap] la sala {pos} dice tener conexion hacia {neighborPos}, pero ahi no existe una sala");
                    isValid = false;
                    continue;
                }

                Vector2Int oppositeDirection = -direction;
                if (!rooms[neighborPos].connections.Contains(oppositeDirection))
                {
                    Debug.LogError($"[ValidateMap] conexion unilateral detectada: {pos} -> {neighborPos} existe, pero {neighborPos} -> {pos} no");
                    isValid = false;
                }
            }
        }

        foreach (var pos in rooms.Keys) // Usar el tipo explícito en lugar de var
        {
            if (!IsInsideGrid(pos))
            {
                Debug.LogError($"[ValidateMap] la sala {pos} quedo fuera del limite de la cuadricula");
                isValid = false;
            }
        }

        isValid &= ValidateSingleType(RoomType.Start, exactlyOne: true);
        isValid &= ValidateSingleType(RoomType.Boss, exactlyOne: false);
        isValid &= ValidateSingleType(RoomType.Treasure, exactlyOne: false);
        isValid &= ValidateSingleType(RoomType.Shop, exactlyOne: false);

        if (isValid)
        {
            Debug.Log("[ValidateMap] mapa vlaido");
        }
    }

    private bool ValidateSingleType(RoomType type, bool exactlyOne)
    {
        int count = 0;
        foreach (var node in rooms.Values) // Usar el tipo explícito en lugar de var
        {
            if (node.type == type)
            {
                count++;
            }

            
        }
        if (exactlyOne && count != 1)
        {
            Debug.LogError($"[ValidateMap] se esperaba exactamente 1 sala del tipo {type}, pero hay {count}");
            return false;
        }

        if (!exactlyOne && count > 1)
        {
            Debug.LogError($"[ValidateMap] no deberia de haber mas de 1 sala de tipo {type}, pero hay {count}");
            return false;
        }

        return true;
    }

    private void InstantiateRooms()
    {
        foreach (var node in rooms.Values) // Usar el tipo explícito en lugar de var
        {
            Vector3 worldPos = new Vector3(node.gridPos.x * roomWorldSize.x, node.gridPos.y * roomWorldSize.y, 0f); // Sin abreviaciones

            GameObject prefab = GetPrefabForType(node.type);
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity, transform);

            Room roomComponent = instance.GetComponent<Room>();
            
            if (roomComponent == null)
            {
                Debug.LogError($"El prefab para {node.type} no tiene el componente Room, la sala en {node.gridPos} no tendra puertas");
                continue;
            }

            bool up = node.connections.Contains(Vector2Int.up); // Revisar: usar un verbo de estado (is, has o can)
            bool down = node.connections.Contains(Vector2Int.down); // Revisar: usar un verbo de estado (is, has o can)
            bool right = node.connections.Contains(Vector2Int.right); // Revisar: usar un verbo de estado (is, has o can)
            bool left = node.connections.Contains(Vector2Int.left); // Revisar: usar un verbo de estado (is, has o can)

            roomComponent.Setup(node.type, node.gridPos, up, down, left, right);
        }
    }

    private GameObject GetPrefabForType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return startRoomPrefab;
            case RoomType.Boss: return bossRoomPrefab; // Separar los case con una línea en blanco
            case RoomType.Treasure: return treasureRoomPrefab; // Separar los case con una línea en blanco
            case RoomType.Shop: return shopRoomPrefab; // Separar los case con una línea en blanco
            default: return normalRoomPrefab; // Separar los case con una línea en blanco
        }
    }

    private void PrintMapToConsole()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        foreach (var kv in rooms) // Usar el tipo explícito en lugar de var
        {
            RoomNode node = kv.Value;
            int distance = distances.ContainsKey(node.gridPos) ? distances[node.gridPos] : -1;
            Debug.Log($"Sala en  {node.gridPos} | tipo: {node.type} | Conexiones: {string.Join(", ", node.connections)} | Distancia: {distance}");
        }
    }
}
