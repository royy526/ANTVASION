using UnityEngine;

public class EnemySpawner_v2 : MonoBehaviour
{
    [Header("Enemigos")]
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private int _minEnemies;
    [SerializeField] private int _maxEnemies;

    [Header("Puntos fijos de aparicion")]
    [SerializeField] private Transform[] _spawnPoints;

    private int _enemiesAlive;
    private bool _waveStarted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_waveStarted)
        {
            _waveStarted = true;
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {

        int count = Random.Range(_minEnemies, _maxEnemies + 1);
        count = Mathf.Min(count, _spawnPoints.Length);
        Debug.Log("Min: " + _minEnemies + " Max: " + _maxEnemies + " Puntos: " + _spawnPoints.Length + " Aparecen: " + count);

        _enemiesAlive = 0;

        for (int i = 0; i < count; i++)
        {

            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];

            GameObject obj = Instantiate(prefab, _spawnPoints[i].position, Quaternion.identity);
            _enemiesAlive++;

            EnemyController_v2 enemy = obj.GetComponent<EnemyController_v2>();
            if (enemy != null)
            {
                enemy.SetSpawner(this);
            }
        }
    }

    public void EnemyDied()
    {
        _enemiesAlive--;
    }
}
