using UnityEngine;
using System.Collections;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance {get; private set;}

    [SerializeField] private MapGenerator _mapGenerator;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _entryOffset = 3f;
    [SerializeField] private float _transitionCooldown = 0.5f;
    private bool _isTransitioning = false;

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerCrossedDoor(Room currentRoom, Vector2Int exitDirection)
    {
        if (_isTransitioning) return;
        Vector2Int neighborPos = currentRoom.GridPos + exitDirection;

        if (!_mapGenerator.InstantiatedRooms.TryGetValue(neighborPos, out Room targetRoom)) 
        {
            Debug.LogWarning($"Se cruzo una puerta hacia {neighborPos} pero no hay nu¿inguna sala instanciada ahi");
            return;
        }
        StartCoroutine(DoTransition(currentRoom, targetRoom, exitDirection));
    }

    private IEnumerator DoTransition(Room currentRoom, Room targetRoom, Vector2Int exitDirection)
    {
        _isTransitioning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        Vector2 roomSize = _mapGenerator.RoomWorldSize;
        Vector3 roomCenter = new Vector3(targetRoom.GridPos.x * roomSize.x, targetRoom.GridPos.y * roomSize.y, 0f);
        Debug.Log($"[Transition] currentRoom.GridPos: {currentRoom.GridPos} | targetRoom.GridPos: {targetRoom.GridPos} | exitDirection: {exitDirection} | roomSize: {roomSize} | roomCenter calculado: {roomCenter}");

        Vector2Int entryDirection = -exitDirection;
        Vector3 entryOffsetVector = new Vector3(entryDirection.x, entryDirection.y, 0f) * _entryOffset;
        Vector3 newPlayerPos = roomCenter + entryOffsetVector;

        Debug.Log($"[transition] player encontrado: {player.name} | posicion ANTES: {player.transform.position} | Destino calculado: {newPlayerPos}");

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
        Debug.Log($"[Transition] Posicion INMEDIATAMENTE DESPUES de asignar: {player.transform.position}");

        yield return null;
        Debug.Log($"[transition] posicion UN FRAME DESPUES: {player.transform.position}");

        _mainCamera.transform.position = new Vector3(roomCenter.x, roomCenter.y, _mainCamera.transform.position.z);
        yield return new WaitForSeconds(_transitionCooldown);
        _isTransitioning = false;
    }
}
