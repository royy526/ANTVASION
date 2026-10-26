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

        if(_doorUp != null) { _doorUp.SetActive(up); }

        if(_doorDown != null) { _doorDown.SetActive(down); }

        if(_doorLeft != null) { _doorLeft.SetActive(left); }

        if(_doorRight != null) { _doorRight.SetActive(right); }
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

