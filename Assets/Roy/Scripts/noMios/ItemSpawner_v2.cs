using UnityEngine;

public class ItemSpawner_v2 : MonoBehaviour
{
    [Header("Enemigos")]
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField] private int _minItems = 1;
    [SerializeField] private int _maxItems = 3;

    [Header("Puntos fijos de aparicion")]
    [SerializeField] private Transform[] _itemSpawnPoints;

    private bool _isSpawned;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isSpawned)
        {
            _isSpawned = true;
            SpawnItems();
        }
    }

    private void SpawnItems()
    {

        int count = Random.Range(_minItems, _maxItems + 1);
        count = Mathf.Min(count, _itemSpawnPoints.Length);


        for (int i = 0; i < count; i++)
        {

            GameObject prefab = _itemPrefabs[Random.Range(0, _itemPrefabs.Length)];

            GameObject obj = Instantiate(prefab, _itemSpawnPoints[i].position, Quaternion.identity);

            CollectionController_v2 item = obj.GetComponent<CollectionController_v2>();
            if (item != null)
            {
                item.SetSpawner(this);
            }
        }
    }

}