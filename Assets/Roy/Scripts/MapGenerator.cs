using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Generacion")]
    [SerializeField] private int _targetRoomCount = 10;
    [SerializeField] private int _minAcceptableRooms = 6;
    [SerializeField] private int _maxAttempts = 300;
    [SerializeField] private int _maxRegenerations = 20;
    [SerializeField] private Vector2Int _gridHalfSize = new Vector2Int(4, 4);
    [SerializeField] [Tooltip("Probabilidad de que despues de cada paso el recorrido salte a una sala ya existente al azar para continuar desde ella")] [Range(0f, 1f)] private float _jumpChance = 0.15f;

    [Header("Salas")]
    [SerializeField] [FormerlySerializedAs("normalRoomPrefab")] private GameObject _normalRoomPrefab;
    [SerializeField] [FormerlySerializedAs("startRoomPrefab")] private GameObject _startRoomPrefab;
    [SerializeField] [FormerlySerializedAs("bossRoomPrefab")] private GameObject _bossRoomPrefab;
    [SerializeField] [FormerlySerializedAs("treasureRoomPrefab")] private GameObject _treasureRoomPrefab;
    [SerializeField] [FormerlySerializedAs("shopRoomPrefab")] private GameObject _shopRoomPrefab;
    [SerializeField] [FormerlySerializedAs("roomWorldSize")] private Vector2 _roomWorldSize = new Vector2(20f, 11f);

    private Dictionary<Vector2Int, RoomNode> _rooms = new Dictionary<Vector2Int, RoomNode>();
    public Dictionary<Vector2Int, Room> InstantiatedRooms {get; private set;} = new Dictionary<Vector2Int, Room>();
    public Vector2 RoomWorldSize => _roomWorldSize;

    private static readonly Vector2Int[] _directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

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
        while (_rooms.Count < _minAcceptableRooms && regenerations < _maxRegenerations);

        if (_rooms.Count < _minAcceptableRooms)
        {
            Debug.LogError($"No se logro generar un mapa valido despues de {_maxRegenerations} intentos, ultimo resultado: {_rooms.Count} salas");
        }
    }

    private void GenerateLayout()
    {
        _rooms.Clear();

        Vector2Int current = Vector2Int.zero;
        RoomNode start = new RoomNode(current) { Type = RoomType.Start };
        _rooms.Add(current, start);
        
        int attempts = 0;

        while (_rooms.Count < _targetRoomCount && attempts < _maxAttempts)
        {
            attempts++;

            Vector2Int direction = _directions[Random.Range(0, _directions.Length)];
            Vector2Int next = current + direction;

            if (!IsInsideGrid(next)) { continue; }

            if (_rooms.ContainsKey(next)) { current = next; }
            else
            {
                if (CountOccupiedNeighbours(next) > 1) { continue; }

                RoomNode newRoom = new RoomNode(next);
                _rooms.Add(next, newRoom);

                ConnectRooms(current, next);

                current = next;
            }

            if (Random.value < _jumpChance)
            {
                List<Vector2Int> keys = new List<Vector2Int>(_rooms.Keys);
                current = keys[Random.Range(0, keys.Count)];
            }
        }
    }

    private bool IsInsideGrid(Vector2Int position)
    {
        return Mathf.Abs(position.x) <= _gridHalfSize.x && Mathf.Abs(position.y) <= _gridHalfSize.y;
    }

    private int CountOccupiedNeighbours(Vector2Int position)
    {
        int count = 0;
        foreach (Vector2Int direction in _directions)
        {
            if (_rooms.ContainsKey(position + direction)) { count++; }
        }
        return count;
    }

    private void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        _rooms[a].Connections.Add(b - a);
        _rooms[b].Connections.Add(a - b);
    }

    private void AssignSpecialRooms()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        List<Vector2Int> leaves = new List<Vector2Int>();
        foreach(KeyValuePair<Vector2Int, RoomNode> entry in _rooms)
        {
            if (entry.Value.Type == RoomType.Normal && entry.Value.Connections.Count == 1) { leaves.Add(entry.Key); }
        }

        if (leaves.Count == 0) { return; }

        leaves.Sort((a, b) => distances[b].CompareTo(distances[a]));

        Vector2Int bossPosition = leaves[0];
        _rooms[bossPosition].Type = RoomType.Boss;
        leaves.RemoveAt(0);

        if (leaves.Count > 0)
        {
            int index = Random.Range(0, leaves.Count);
            _rooms[leaves[index]].Type = RoomType.Treasure;
            leaves.RemoveAt(index);
        }

        if (leaves.Count > 0)
        {
            int index = Random.Range(0, leaves.Count);
            _rooms[leaves[index]].Type = RoomType.Shop;
        }
    }

    private Dictionary<Vector2Int, int> ComputeDistanceFromStart()
    {
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int startPosition = Vector2Int.zero;
        distances[startPosition] = 0;
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach(Vector2Int direction in _rooms[current].Connections)
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

        isValid &= ValidateConnections();
        isValid &= ValidateGridBounds();
        isValid &= ValidateSpecialRoomsCounts();
    }

    private bool ValidateConnections()
    {
        bool isValid = true;

        foreach (KeyValuePair<Vector2Int, RoomNode> entry in _rooms)
        {
            Vector2Int position = entry.Key;
            RoomNode node = entry.Value;

            foreach (Vector2Int direction in node.Connections)
            {
                Vector2Int neighborPosition = position + direction;

                if (!_rooms.ContainsKey(neighborPosition))
                {
                    isValid = false;
                    continue;
                }

                Vector2Int oppositeDirection = -direction;

                if (!_rooms[neighborPosition].Connections.Contains(oppositeDirection)) { isValid = false; }
            }
        }

        return isValid;
    }

    private bool ValidateGridBounds()
    {
        bool isValid = true;

        foreach (Vector2Int position in _rooms.Keys)
        {
            if (!IsInsideGrid(position)) { isValid = false; }

            return isValid;
        }

        return isValid;
    }

    private bool ValidateSpecialRoomsCounts()
    {
        bool isValid = true;

        isValid &= ValidateSingleType(RoomType.Start, exactlyOne: true);
        isValid &= ValidateSingleType(RoomType.Boss, exactlyOne: true);
        isValid &= ValidateSingleType(RoomType.Treasure, exactlyOne: true);
        isValid &= ValidateSingleType(RoomType.Shop, exactlyOne: true);

        return isValid;
    }

    private bool ValidateSingleType(RoomType type, bool exactlyOne)
    {
        int count = 0;

        foreach(RoomNode node in _rooms.Values)
        {
            if (node.Type == type)
            {
                count++;
            }

            if (exactlyOne && count != 1) { return false; }

            if (!exactlyOne && count > 1) { return false; }
        }

        return true;
    }

    private void InstantiateRooms()
    {
        foreach(RoomNode node in _rooms.Values)
        {
            Vector3 worldPosition = new Vector3(node.GridPosition.x * _roomWorldSize.x, node.GridPosition.y * _roomWorldSize.y, 0f);

            GameObject prefab = GetPrefabForType(node.Type);
            GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity, transform);

            Room roomComponent = instance.GetComponent<Room>();
            
            if (roomComponent == null)
            {
                Debug.LogError($"El prefab para {node.Type} no tiene el componente Room, la sala en {node.GridPosition} no tendra puertas");
                continue;
            }

            bool isConnectedUp = node.Connections.Contains(Vector2Int.up);
            bool isConnectedDown = node.Connections.Contains(Vector2Int.down);
            bool isConnectedRight = node.Connections.Contains(Vector2Int.right);
            bool isConnectedLeft = node.Connections.Contains(Vector2Int.left);

            roomComponent.Setup(node.Type, node.GridPosition, isConnectedUp, isConnectedDown, isConnectedLeft, isConnectedRight);

            InstantiatedRooms[node.GridPosition] = roomComponent;
        }
    }

    private GameObject GetPrefabForType(RoomType type)
    {
        switch(type)
        {
            case RoomType.Start: return _startRoomPrefab;
            case RoomType.Boss: return _bossRoomPrefab;
            case RoomType.Treasure: return _treasureRoomPrefab;
            case RoomType.Shop: return _shopRoomPrefab;
            default: return _normalRoomPrefab;
        }
    }

    private void PrintMapToConsole()
    {
        Dictionary<Vector2Int, int> distances = ComputeDistanceFromStart();

        foreach (KeyValuePair<Vector2Int, RoomNode> entry in _rooms)
        {
            RoomNode node = entry.Value;
            int distance = distances.ContainsKey(node.GridPosition) ? distances[node.GridPosition] : -1;
        }
    }
}
