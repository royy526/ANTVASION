// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;

public class OLDEnemySpawner : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player; // Usar PascalCase

    [Header("Enemigos")]
    public GameObject[] enemyPrefabs; // Usar PascalCase

    [Header("Cantidad de enemigos")]
    public int minEnemies = 3; // Usar PascalCase
    public int maxEnemies = 6; // Usar PascalCase

    [Header("Zona de aparición")]
    public float spawnDistance = 5f; // Usar PascalCase; Aclarar el significado o las unidades con Tooltip

    private int enemiesAlive; // Usar _camelCase
    private bool waveStarted = false; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)
    private bool waveCompleted = false; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)

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
            (Vector2)player.position + randomDirection * spawnDistance; // Aclarar el significado o las unidades con Tooltip

        int randomEnemy = Random.Range(0, enemyPrefabs.Length);

        GameObject enemyObject = Instantiate(
            enemyPrefabs[randomEnemy],
            spawnPosition,
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (enemy != null)
        {
           // enemy.SetSpawner(this);
        }

        enemiesAlive++;
    }

    public void EnemyDied() // Respetar el orden de acceso y static
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
