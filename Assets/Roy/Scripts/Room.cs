// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;

public enum RoomType {Start, Normal, Boss, Treasure, Shop, Secret} // Usar llaves en estilo Allman; Mover el enum público a su propio archivo
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
    private bool _roomStarted; // Revisar: usar un verbo de estado (is, has o can); Respetar el orden de los miembros
    private bool _roomCleared; // Revisar: usar un verbo de estado (is, has o can); Respetar el orden de los miembros

    [SerializeField] private EnemySpawner _spawner; // Respetar el orden de los miembros

    [SerializeField] private bool _hasEnemies = true; // Respetar el orden de los miembros


    public void Setup(RoomType type, Vector2Int gridPos, // Sin abreviaciones
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


    private void Awake() // Respetar el orden de los miembros
    {
        if (_spawner != null)
        {
            _spawner.AllEnemiesDefeated += ClearRoom;
        }
    }

    private void OnDestroy() // Respetar el orden de los miembros
    {
        if (_spawner != null)
        {
            _spawner.AllEnemiesDefeated -= ClearRoom;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Respetar el orden de los miembros
    {
        if (_roomStarted || _roomCleared) return; // Agregar llaves al if
        if (!_hasEnemies) return; // Agregar llaves al if
        if (!other.CompareTag("Player")) return; // Agregar llaves al if

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
        if (_upConnected) SetBlocker(_doorUpBlocker, locked); // Agregar llaves al if
        if (_downConnected) SetBlocker(_doorDownBlocker, locked); // Agregar llaves al if
        if (_leftConnected) SetBlocker(_doorLeftBlocker, locked); // Agregar llaves al if
        if (_rightConnected) SetBlocker(_doorRightBlocker, locked); // Agregar llaves al if
    }

    private void SetBlocker(Collider2D blocker, bool active)
    {
        if (blocker == null)
        {
            return;
        }

        blocker.enabled = active;
    }

    private void OnValidate() // Respetar el orden de los miembros
    {
        if (_doorUpBlocker == null || _doorDownBlocker == null ||
            _doorLeftBlocker == null || _doorRightBlocker == null)
        {
        }
    }

    public void OnPlayerEnteredZone()
    {
        if (_roomStarted || _roomCleared || !_hasEnemies) return; // Agregar llaves al if

        StartRoom();
    }
}

