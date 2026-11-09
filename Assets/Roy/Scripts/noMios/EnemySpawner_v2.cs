// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemySpawner_v2 : MonoBehaviour // Usar PascalCase
{
    [Header("Enemigos")]
    [SerializeField] private List<Enemy_v2> _enemyPrefabs;
    [SerializeField] private int _minEnemies = 2; // Sin abreviaciones
    [SerializeField] private int _maxEnemies = 4; // Sin abreviaciones

    [Header("Posiciones de aparicion")]
    [SerializeField] private Transform[] _spawnPoints;

    private int _enemiesAlive;

    public event Action AllEnemiesDefeated;


    public bool Spawn()
    {
        if (_enemyPrefabs.Count == 0 || _spawnPoints.Length == 0)
        {
            return false;
        }

        int count = UnityEngine.Random.Range(_minEnemies, _maxEnemies + 1);
        count = Mathf.Min(count, _spawnPoints.Length);


        List<Transform> points = new List<Transform>(_spawnPoints);
        for (int i = points.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // Usar un nombre descriptivo para j
            (points[i], points[j]) = (points[j], points[i]);
        }

        _enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            Enemy_v2 prefab = _enemyPrefabs[UnityEngine.Random.Range(0, _enemyPrefabs.Count)];
            Enemy_v2 enemy = Instantiate(prefab, points[i].position, Quaternion.identity, transform.parent);
            enemy.SetSpawner(this);
        }

        return count > 0;
    }


    public void EnemyDied()
    {
        _enemiesAlive--;

        if (_enemiesAlive <= 0)
        {
            OnAllEnemiesDefeated();
        }
    }

    protected virtual void OnAllEnemiesDefeated()
    {
        AllEnemiesDefeated?.Invoke();
    }
}
