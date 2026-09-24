using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider))]
public class EnemyRoomEncounter : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField, Min(1)] private int enemiesToSpawn = 3;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Grid")]
    [SerializeField, Min(0.25f)] private float cellSize = 0.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Room barriers")]
    [SerializeField] private bool createPrototypeBarriers = true;
    [SerializeField] private UnityEvent onRoomCleared;

    private readonly List<RoomEnemy> aliveEnemies = new();
    private readonly List<GameObject> barriers = new();
    private BoxCollider roomBounds;
    private EnemySpawnGrid spawnGrid;
    private bool encounterStarted;
    private bool roomCleared;

    public int EnemiesAlive => aliveEnemies.Count;
    public bool RoomCleared => roomCleared;

    private void Awake()
    {
        roomBounds = GetComponent<BoxCollider>();
        FindPlayer();
        EnsurePlayerPhysics();
        CreateGrid();

        if (createPrototypeBarriers)
            CreatePrototypeBarriers();

        SetBarriers(false);
    }

    private void Update()
    {
        if (encounterStarted || player == null)
            return;

        if (roomBounds.bounds.Contains(player.position))
            StartEncounter();
    }

    public void StartEncounter()
    {
        if (encounterStarted)
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

        if (aliveEnemies.Count == 0)
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
            if (selectedPrefab == null)
                return;
            enemyObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);
        }






    }


    private void HandleEnemyDied(RoomEnemy enemy)
    {
        enemy.Died -= HandleEnemyDied;
        aliveEnemies.Remove(enemy);
        if (aliveEnemies.Count == 0)
            CompleteRoom();
    }

    private void CompleteRoom()
    {
        if (roomCleared)
            return;

        roomCleared = true;
        SetBarriers(false);
        onRoomCleared?.Invoke();
    }

    private void FindPlayer()
    {
        if (player != null)
            return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    private void EnsurePlayerPhysics()
    {
        if (player == null)
            return;

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody == null)
        {
            playerBody = player.gameObject.AddComponent<Rigidbody2D>();
            playerBody.gravityScale = 0f;
            playerBody.freezeRotation = true;
        }

        if (player.GetComponent<Collider2D>() == null)
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
        CreateBarrier("North Barrier", new Vector2(bounds.center.x, bounds.max.y), new Vector2(bounds.size.x, 0.35f));
        CreateBarrier("South Barrier", new Vector2(bounds.center.x, bounds.min.y), new Vector2(bounds.size.x, 0.35f));
        CreateBarrier("East Barrier", new Vector2(bounds.max.x, bounds.center.y), new Vector2(0.35f, bounds.size.y));
        CreateBarrier("West Barrier", new Vector2(bounds.min.x, bounds.center.y), new Vector2(0.35f, bounds.size.y));
    }

    private void CreateBarrier(string barrierName, Vector2 position, Vector2 size)
    {
        GameObject barrier = new GameObject(barrierName);
        barrier.transform.SetParent(transform);
        barrier.transform.position = position;

        SpriteRenderer renderer = barrier.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSquareSprite();
        renderer.color = new Color(0.2f, 0.85f, 1f, 0.8f);
        barrier.transform.localScale = new Vector3(size.x, size.y, 1f);

        BoxCollider2D collider = barrier.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        barriers.Add(barrier);
    }

    private static Sprite CreateSquareSprite()
    {
        return Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    private void SetBarriers(bool closed)
    {
        foreach (GameObject barrier in barriers)
            barrier.SetActive(closed);
    }
}
