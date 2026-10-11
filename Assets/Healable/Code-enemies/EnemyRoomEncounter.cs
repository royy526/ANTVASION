using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;




public class EnemyRoomEncounter : MonoBehaviour

{
    [Header("Encounter")]
    [SerializeField, Min(1)] private int enemiesToSpawn = 3; // Usar _camelCase
    [SerializeField] private Transform player; // Usar _camelCase
    [SerializeField] private GameObject[] enemyPrefabs; // Usar _camelCase

    [Header("Grid")]
    [SerializeField, Min(0.25f)] private float cellSize = 0.5f; // Usar _camelCase
    [SerializeField] private LayerMask obstacleMask; // Usar _camelCase

    [Header("Room barriers")]
    [SerializeField] private bool createPrototypeBarriers = true; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)
    [SerializeField] private UnityEvent onRoomCleared; // Usar _camelCase

    private readonly List<RoomEnemy> aliveEnemies = new(); // Usar _camelCase
    private readonly List<GameObject> barriers = new(); // Usar _camelCase
    [SerializeField] private BoxCollider2D roomBounds; // Usar _camelCase
    private EnemySpawnGrid spawnGrid; // Usar _camelCase
    private bool encounterStarted; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)
    private bool roomCleared; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)

    public int EnemiesAlive => aliveEnemies.Count;
    public bool RoomCleared => roomCleared;

    [Range(0f, 1f)]
    public float myAlpha = 0f; // Usar PascalCase; Respetar el orden de los miembros

    private void Awake()
    {
        FindPlayer();
        EnsurePlayerPhysics(); // Revisar: una sola responsabilidad
        CreateGrid();

        if (createPrototypeBarriers) // Agregar llaves al if
            CreatePrototypeBarriers();

        SetBarriers(false);
    }

    private void Update()
    {
        if (encounterStarted || player == null) // Agregar llaves al if
            return;

        if (roomBounds.bounds.Contains(player.position)) // Agregar llaves al if
            StartEncounter();
    }

    public void StartEncounter()
    {
        if (encounterStarted) // Agregar llaves al if
            return;

        FindPlayer();
        if (player == null)
        {
            Debug.LogWarning("Enemy room needs a GameObject tagged Player.", this);
            return;
        }

        encounterStarted = true;
        SetBarriers(true);
        for (int i = 0; i < enemiesToSpawn; i++)
            SpawnEnemy();

        if (aliveEnemies.Count == 0) // Agregar llaves al if
            CompleteRoom();
    }

    private void SpawnEnemy()
    {
        if (!spawnGrid.TryGetRandomFreeCell(out Vector2 spawnPosition))
        {
            Debug.LogWarning("There is no free grid cell for another enemy.", this);
            return;
        }

        GameObject enemyObject;
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (selectedPrefab == null) // Agregar llaves al if
                return;
            enemyObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);
        }






    }


    private void HandleEnemyDied(RoomEnemy enemy)
    {
        enemy.Died -= HandleEnemyDied;
        aliveEnemies.Remove(enemy);
        if (aliveEnemies.Count == 0) // Agregar llaves al if
            CompleteRoom();
    }

    private void CompleteRoom()
    {
        if (roomCleared) // Agregar llaves al if
            return;

        roomCleared = true;
        SetBarriers(false);
        onRoomCleared?.Invoke(); // Invocar el evento desde un método On...
    }

    private void FindPlayer()
    {
        if (player != null) // Agregar llaves al if
            return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) // Agregar llaves al if
            player = playerObject.transform;
    }

    private void EnsurePlayerPhysics()
    {
        if (player == null) // Agregar llaves al if
            return;

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody == null)
        {
            playerBody = player.gameObject.AddComponent<Rigidbody2D>();
            playerBody.gravityScale = 0f;
            playerBody.freezeRotation = true;
        }

        if (player.GetComponent<Collider2D>() == null) // Agregar llaves al if
            player.gameObject.AddComponent<BoxCollider2D>();
    }

    private void CreateGrid()
    {
        GameObject gridObject = new GameObject("Enemy Spawn Grid", typeof(Grid), typeof(EnemySpawnGrid));
        gridObject.transform.SetParent(transform, false);
        spawnGrid = gridObject.GetComponent<EnemySpawnGrid>();
        spawnGrid.Configure(roomBounds.bounds, cellSize, obstacleMask);
    }

    private void CreatePrototypeBarriers()
    {
        Bounds bounds = roomBounds.bounds;
        CreateBarrier("North Barrier", new Vector2(bounds.center.x, bounds.max.y), new Vector2(bounds.size.x, 0.35f)); // Revisar: usar una constante o configuración con nombre
        CreateBarrier("South Barrier", new Vector2(bounds.center.x, bounds.min.y), new Vector2(bounds.size.x, 0.35f)); // Revisar: usar una constante o configuración con nombre
        CreateBarrier("East Barrier", new Vector2(bounds.max.x, bounds.center.y), new Vector2(0.35f, bounds.size.y));
        CreateBarrier("West Barrier", new Vector2(bounds.min.x, bounds.center.y), new Vector2(0.35f, bounds.size.y));
    }

    private void CreateBarrier(string barrierName, Vector2 position, Vector2 size)
    {
        GameObject barrier = new GameObject(barrierName);
        barrier.transform.position = position;

        

        SpriteRenderer renderer = barrier.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSquareSprite();
        renderer.color = new Color(0.2f, 0.85f, 1f, 0.8f);
        barrier.transform.localScale = new Vector3(size.x, size.y, 1f);
        BoxCollider2D collider = barrier.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        barriers.Add(barrier);
        
    }

    private static Sprite CreateSquareSprite() // Respetar el orden de acceso y static
    {
        return Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    private void SetBarriers(bool closed)
    {
        foreach (GameObject barrier in barriers)
            barrier.SetActive(closed);
    }
}
