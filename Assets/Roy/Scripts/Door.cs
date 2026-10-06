using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector2Int _direction;
    private Room _ownerRoom;

    void Awake()
    {
        _ownerRoom = GetComponentInParent<Room>();

        if (_ownerRoom == null)
        {
            Debug.LogError($"{name}: no se encontro un componente room en los padres de esta puerta");
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RoomTransitionManager.Instance.OnPlayerCrossedDoor(_ownerRoom, _direction);
    }
}
