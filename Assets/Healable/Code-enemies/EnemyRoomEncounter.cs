using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;




public class EnemyRoomEncounter : MonoBehaviour

{
    [Header("Encounter")]
    [SerializeField, Min(1)] private int _enemiesToSpawn = 3;
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject[] _enemyPrefabs;

    [Header("Grid")]
    [SerializeField, Min(0.25f)] private float _cellSize = 0.5f;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Room barriers")]
    [SerializeField] private bool _canCreatePrototypeBarriers = true;
    [SerializeField] private UnityEvent _onRoomCleared;

    private readonly List<RoomEnemy> _aliveEnemies = new();
    private readonly List<GameObject> _barriers = new();
    [SerializeField] private BoxCollider2D _roomBounds;
    private EnemySpawnGrid _spawnGrid;
    private bool _hasEncounterStarted;
    private bool _isRoomCleared;

    public int EnemiesAlive => _aliveEnemies.Count;
    public bool RoomCleared => _isRoomCleared;


    private void Awake()
    {
        FindPlayer();
        EnsurePlayerRigidbody();
        EnsurePlayerCollider();
        CreateGrid();

        if (_canCreatePrototypeBarriers)
            CreatePrototypeBarriers();

        SetBarriers(false);
    }

    private void Update()
    {
        if (_hasEncounterStarted || _player == null)
        {
            return;
        }

        if (_roomBounds.bounds.Contains(_player.position))
        {
            StartEncounter();
        }
    }

    public void StartEncounter()
    {
        if (_hasEncounterStarted)
        {
            return;
        }

        FindPlayer();
        if (_player == null)
        {
            Debug.LogWarning("Enemy room needs a GameObject tagged Player.", this);
            return;
        }

        _hasEncounterStarted = true;
        SetBarriers(true);
        for (int i = 0; i < _enemiesToSpawn; i++)
            SpawnEnemy();

        if (_aliveEnemies.Count == 0)
        {
            CompleteRoom();
        }
    }

    private void SpawnEnemy()
    {
        if (!_spawnGrid.TryGetRandomFreeCell(out Vector2 spawnPosition))
        {
            Debug.LogWarning("There is no free grid cell for another enemy.", this);
            return;
        }

        GameObject enemyObject;
        if (_enemyPrefabs != null && _enemyPrefabs.Length > 0)
        {
            GameObject selectedPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            if (selectedPrefab == null)
            {
                return;
            }
            enemyObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }


    private void HandleEnemyDied(RoomEnemy enemy)
    {
        enemy.Died -= HandleEnemyDied;
        _aliveEnemies.Remove(enemy);
        if (_aliveEnemies.Count == 0)
        {
            CompleteRoom();
        }
    }

    private void CompleteRoom()
    {
        if (_isRoomCleared)
        {
            return;
        }

        _isRoomCleared = true;
        SetBarriers(false);
        _onRoomCleared?.Invoke();
    }

    private void FindPlayer()
    {
        if (_player != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    private void EnsurePlayerRigidbody()
    {
        if (_player == null)
        {
            return;
        }

        Rigidbody2D playerBody = _player.GetComponent<Rigidbody2D>();
        if (playerBody == null)
        {
            playerBody = _player.gameObject.AddComponent<Rigidbody2D>();
            playerBody.gravityScale = 0f;
            playerBody.freezeRotation = true;
        }
    }

    private void EnsurePlayerCollider()
    {
        if (_player == null)
        {
            return;
        }

        if (_player.GetComponent<Collider2D>() == null)
        {
            _player.gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void CreateGrid()
    {
        GameObject gridObject = new GameObject("Enemy Spawn Grid", typeof(Grid), typeof(EnemySpawnGrid));
        gridObject.transform.SetParent(transform, false);
        _spawnGrid = gridObject.GetComponent<EnemySpawnGrid>();
        _spawnGrid.Configure(_roomBounds.bounds, _cellSize, _obstacleMask);
    }

    private void CreatePrototypeBarriers()
    {
        Bounds bounds = _roomBounds.bounds;
        CreateBarrier("North Barrier", new Vector2(bounds.center.x, bounds.max.y), new Vector2(bounds.size.x, 0.35f));
        CreateBarrier("South Barrier", new Vector2(bounds.center.x, bounds.min.y), new Vector2(bounds.size.x, 0.35f));
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
        _barriers.Add(barrier);

    }

    private static Sprite CreateSquareSprite()
    {
        return Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    private void SetBarriers(bool closed)
    {
        foreach (GameObject barrier in _barriers)
            barrier.SetActive(closed);
    }
}