using UnityEngine;
using System.Collections.Generic;

public class MapGeneratos : MonoBehaviour
{
    [Header("Generacion")]
    [SerializeField] private int _targetRoomCount = 10;
    [SerializeField] private int _minAcceptableRooms = 6;
    [SerializeField] private int _maxAttempts = 300;
    [SerializeField] private int _maxRegenerations = 20;
    [SerializeField] private Vector2Int _gridHalfSize = new Vector2Int(4, 4);
    [SerializeField] [Range(0f, 1f)] private float _jumpChance = 0.15f;

    [Header("Salas")]
    [SerializeField] private GameObject normalRoomPrefab;
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private GameObject treasureRoomPrefab;
    [SerializeField] private GameObject shopRoomPrefab;
    [SerializeField] private Vector2 roomWorldSize = new Vector2(20f, 11f);

    private Dictionary<Vector2Int, RoomNode> rooms = new Dictionary<Vector2Int, RoomNode>();

    private static readonly Vector2Int[] Directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

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
        var start = new RoomNode(current) {type = RoomType.Start};
        rooms.Add(current, start);
        
        int attempts = 0;

        while (rooms.Count < _targetRoomCount && attempts < _maxAttempts)
        {
            attempts++;

            Vector2Int direction = Directions[Random.Range(0, Directions.Length)];
            Vector2Int next = current + direction;

            if (!IsInsideGrid(next))
            continue;

            if (rooms.ContainsKey(next))
            {
                current = next;
            }
            else
            {
                if (CountOccupiedNeighbours(next) > 1)
                continue;

                var newRoom = new RoomNode(next);
                rooms.Add(next, newRoom);

                ConnectRooms(current, next);

                current = next;
            }

            if (Random.value < _jumpChance)
            {
                var keys = new List<Vector2Int>(rooms.Keys);
                current = keys[Random.Range(0, keys.Count)];
            }
        }
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return Mathf.Abs(pos.x) <= _gridHalfSize.x && Mathf.Abs(pos.y) <= _gridHalfSize.y;
    }

    private int CountOccupiedNeighbours(Vector2Int pos)
    {
        int count = 0;
        foreach (var dir in Directions)
        {
            if (rooms.ContainsKey(pos + dir))
            count++;
        }
        return count;
    }

    private void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        rooms[a].connections.Add(b-a);
        rooms[b].connections.Add(a-b);
    }

    private void AssignSpecialRooms()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        List<Vector2Int> leaves = new List<Vector2Int>();
        foreach(var kv in rooms)
        {
            if (kv.Value.type == RoomType.Normal && kv.Value.connections.Count == 1)
            leaves.Add(kv.Key);
        }

        if (leaves.Count == 0)
        {
            Debug.LogWarning("No hay salas hoja disponibles y el mapa se quedada sin salas especiales");
            return;
        }

        leaves.Sort((a, b) => distances[b].CompareTo(distances[a]));

        Vector2Int bossPos = leaves[0];
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
        var distances = new Dictionary<Vector2Int, int>();
        var queue = new Queue<Vector2Int>();

        Vector2Int startPos = Vector2Int.zero;
        distances[startPos] = 0;
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach(var direction in rooms[current].connections)
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

    private void ValidateMap()
    {
        bool isValid = true;

        foreach(var kv in rooms)
        {
            Vector2Int pos = kv.Key;
            RoomNode node = kv.Value;

            foreach(var direction in node.connections)
            {
                Vector2Int neighborPos = pos + direction;

                if (!rooms.ContainsKey(neighborPos))
                {
                    Debug.LogError($"[ValidateMap] la sala {pos} dice tener conexion hacia {neighborPos}, pero ahi no existe una sala");
                    isValid = false;
                    continue;
                }

                Vector2Int oppositeDirection = -direction;
                if(!rooms[neighborPos].connections.Contains(oppositeDirection))
                {
                    Debug.LogError($"[ValidateMap] conexion unilateral detectada: {pos} -> {neighborPos} existe, pero {neighborPos} -> {pos} no");
                    isValid = false;
                }
            }
        }

        foreach(var pos in rooms.Keys)
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
            Debug.Log("[ValidateMap] mapa vlaido: todas las verificaciones pasaron");
        }
    }

    private bool ValidateSingleType(RoomType type, bool exactlyOne)
    {
        int count = 0;
        foreach(var node in rooms.Values)
        {
            if (node.type == type)
            {
                count++;
            }

            if (exactlyOne && count !=1)
            {
                Debug.LogError($"[ValidateMap] se esperaba exactamente 1 sala del tipo {type}, pero hay {count}");
                return false;
            }

            if (!exactlyOne && count > 1)
            {
                Debug.LogError($"[ValidateMap] no deberia de haber mas de 1 sala de tipo {type}, pero hay {count}");
                return false;
            }
        }

        return true;
    }

    private void InstantiateRooms()
    {
        foreach(var node in rooms.Values)
        {
            Vector3 worldPos = new Vector3(node.gridPos.x * roomWorldSize.x, node.gridPos.y * roomWorldSize.y, 0f);

            GameObject prefab = GetPrefabForType(node.type);
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity, transform);

            Room roomComponent = instance.GetComponent<Room>();
            
            if (roomComponent == null)
            {
                Debug.LogError($"El prefab para {node.type} no tiene el componente Room, la sala en {node.gridPos} no tendra puertas");
                continue;
            }

            bool up = node.connections.Contains(Vector2Int.up);
            bool down = node.connections.Contains(Vector2Int.down);
            bool right = node.connections.Contains(Vector2Int.right);
            bool left = node.connections.Contains(Vector2Int.left);

            roomComponent.Setup(node.type, node.gridPos, up, down, left, right);
        }

        Debug.Log($"[InstantiateRooms] termino. se crearon {transform.childCount} salas como hijas de MapGenerator");
    }

    private GameObject GetPrefabForType(RoomType type)
    {
        switch(type)
        {
            case RoomType.Start: return startRoomPrefab;
            case RoomType.Boss: return bossRoomPrefab;
            case RoomType.Treasure: return treasureRoomPrefab;
            case RoomType.Shop: return shopRoomPrefab;
            default: return normalRoomPrefab;
        }
    }

    private void PrintMapToConsole()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        foreach (var kv in rooms)
        {
            RoomNode node = kv.Value;
            int distance = distances.ContainsKey(node.gridPos) ? distances[node.gridPos] : -1;
            Debug.Log($"Sala en  {node.gridPos} | tipo: {node.type} | Conexiones: {string.Join(", ", node.connections)} | Distancia: {distance}");
        }
    }
}
