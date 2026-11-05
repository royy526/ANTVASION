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

    private readonly List<RoomEnemy> aliveEnemies = new();
    private readonly List<GameObject> barriers = new();
    [SerializeField] private BoxCollider2D roomBounds;
    private bool encounterStarted;
    private bool roomCleared;

    public int EnemiesAlive => _aliveEnemies.Count;
    public bool RoomCleared => _isRoomCleared;


    private void Awake()
    {
        FindPlayer();
        EnsurePlayerPhysics();

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

        
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            GameObject selectedPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            if (selectedPrefab == null)
            {
                return;
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




    private void SetBarriers(bool closed)
    {
        foreach (GameObject barrier in _barriers)
            barrier.SetActive(closed);
    }
}