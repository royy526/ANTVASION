using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Cantidad de enemigos")]
    public int minEnemies = 3;
    public int maxEnemies = 6;

    [Header("Zona de aparición")]
    public float spawnDistance = 5f;

    private int enemiesAlive;
    private bool waveStarted = false;
    private bool waveCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !waveStarted)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        waveStarted = true;

        int enemyAmount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            return;
        }

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 spawnPosition =
            (Vector2)player.position + randomDirection * spawnDistance;

        int randomEnemy = Random.Range(0, enemyPrefabs.Length);

        GameObject enemyObject = Instantiate(
            enemyPrefabs[randomEnemy],
            spawnPosition,
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetSpawner(this);
        }

        enemiesAlive++;
    }

    public void EnemyDied()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            CompleteWave();
        }
    }

    private void CompleteWave()
    {
        waveCompleted = true;

    }
}
