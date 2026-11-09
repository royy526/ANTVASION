using UnityEngine;
using System.Collections;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance {get; private set;}

    [SerializeField] private MapGenerator _mapGenerator; // Respetar el orden de los miembros
    [SerializeField] private Camera _mainCamera; // Respetar el orden de los miembros
    [SerializeField] private float _entryOffset = 3f; // Respetar el orden de los miembros
    [SerializeField] private float _transitionCooldown = 0.5f; // Respetar el orden de los miembros
    private bool _isTransitioning = false; // Respetar el orden de los miembros

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerCrossedDoor(Room currentRoom, Vector2Int exitDirection)
    {
        if (_isTransitioning) return; // Agregar llaves al if
        Vector2Int neighborPos = currentRoom.GridPos + exitDirection; // Sin abreviaciones

        if (!_mapGenerator.InstantiatedRooms.TryGetValue(neighborPos, out Room targetRoom)) { return; }
        StartCoroutine(DoTransition(currentRoom, targetRoom, exitDirection));
    }

    private IEnumerator DoTransition(Room currentRoom, Room targetRoom, Vector2Int exitDirection)
    {
        _isTransitioning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>(); // Sin abreviaciones

        Vector2 roomSize = _mapGenerator.RoomWorldSize;
        Vector3 roomCenter = new Vector3(targetRoom.GridPos.x * roomSize.x, targetRoom.GridPos.y * roomSize.y, 0f);

        Vector2Int entryDirection = -exitDirection;
        Vector3 entryOffsetVector = new Vector3(entryDirection.x, entryDirection.y, 0f) * _entryOffset;
        Vector3 newPlayerPos = roomCenter + entryOffsetVector; // Sin abreviaciones


        if (playerRb != null)
        {
            playerRb.position = newPlayerPos;
            playerRb.linearVelocity = Vector2.zero;
            Physics2D.SyncTransforms();
        }
        else
        {
            player.transform.position = newPlayerPos;
        }

        yield return null;

        _mainCamera.transform.position = new Vector3(roomCenter.x, roomCenter.y, _mainCamera.transform.position.z);
        yield return new WaitForSeconds(_transitionCooldown);
        _isTransitioning = false;
    }
}
