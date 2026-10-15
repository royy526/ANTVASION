using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomLockZone : MonoBehaviour
{
    private Room _room;

    private void Awake()
    {
        _room = GetComponentInParent<Room>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_room == null) return; // Agregar llaves al if
    
        if (!other.TryGetComponent<PlayerController>(out _)) return; // Agregar llaves al if

        _room.OnPlayerEnteredZone();
    
}
}