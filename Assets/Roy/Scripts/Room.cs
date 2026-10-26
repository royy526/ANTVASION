using UnityEngine;

public enum RoomType {Start, Normal, Boss, Treasure, Shop, Secret}
public class Room : MonoBehaviour
{

    [Header("Puertas")]
    [SerializeField] private Collider2D _doorUpBlocker;
    [SerializeField] private Collider2D _doorDownBlocker;
    [SerializeField] private Collider2D _doorLeftBlocker;
    [SerializeField] private Collider2D _doorRightBlocker;

    public RoomType RoomType { get; private set; }
    public Vector2Int GridPos { get; private set; }

    private bool _upConnected, _downConnected, _leftConnected, _rightConnected;
    private bool _roomStarted;
    private bool _roomCleared;

    [SerializeField] private EnemySpawner _spawner;

    [SerializeField] private bool _hasEnemies = true;


    public void Setup(RoomType type, Vector2Int gridPos,
                      bool up, bool down, bool left, bool right)
    {
        RoomType = type;
        GridPos = gridPos;

        _upConnected = up;
        _downConnected = down;
        _leftConnected = left;
        _rightConnected = right;


        SetBlocker(_doorUpBlocker, !up);
        SetBlocker(_doorDownBlocker, !down);
        SetBlocker(_doorLeftBlocker, !left);
        SetBlocker(_doorRightBlocker, !right);

    }


    private void Awake()
    {
        if (_spawner != null)
        {
            _spawner.AllEnemiesDefeated += ClearRoom;
        }
    }

    private void OnDestroy()
    {
        if (_spawner != null)
        {
            _spawner.AllEnemiesDefeated -= ClearRoom;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_roomStarted || _roomCleared) return;
        if (!_hasEnemies) return;
        if (!other.CompareTag("Player")) return;

        StartRoom();

    }

    private void StartRoom()
    {
        _roomStarted = true;


        if (_spawner == null || !_spawner.Spawn())
        {
            _roomCleared = true;
            return;
        }

        SetConnectedDoorsLocked(true);

    }

    private void ClearRoom()
    {
        _roomCleared = true;
        SetConnectedDoorsLocked(false);
    }

    private void SetConnectedDoorsLocked(bool locked)
    {
        if (_upConnected) SetBlocker(_doorUpBlocker, locked);
        if (_downConnected) SetBlocker(_doorDownBlocker, locked);
        if (_leftConnected) SetBlocker(_doorLeftBlocker, locked);
        if (_rightConnected) SetBlocker(_doorRightBlocker, locked);
    }

    private void SetBlocker(Collider2D blocker, bool active)
    {
        if (blocker == null)
        {
            return;
        }

        blocker.enabled = active;
    }

    private void OnValidate()
    {
        if (_doorUpBlocker == null || _doorDownBlocker == null ||
            _doorLeftBlocker == null || _doorRightBlocker == null)
        {
        }
    }

    public void OnPlayerEnteredZone()
    {
        if (_roomStarted || _roomCleared || !_hasEnemies) return;

        StartRoom();
    }
}

