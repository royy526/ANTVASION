using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




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
    [SerializeField] private BoxCollider2D roomBounds;
    private bool encounterStarted;
    private bool roomCleared;

    public int EnemiesAlive => aliveEnemies.Count;
    public bool RoomCleared => roomCleared;

    private void Awake()
    {
        FindPlayer();
        EnsurePlayerPhysics();

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

        
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (selectedPrefab == null)
                return;
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




    private void SetBarriers(bool closed)
    {
        foreach (GameObject barrier in barriers)
            barrier.SetActive(closed);
    }
}
