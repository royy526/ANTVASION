using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public struct Spawnable
    {
        public GameObject gameObject;
        public float Weight;
    }
    public List<Spawnable> Items = new List<Spawnable>();
    float totalWeight;
    void Awake()
    {
        totalWeight = 0;
        foreach (var spawnable in Items)
        {
            totalWeight += spawnable.Weight;
        }
    }
    void Start()
    {
        float pick = Random.value * totalWeight;
        int chosenIndex = 0;
        float cumulativeWeight = Items[0].Weight;

        while (pick > cumulativeWeight && chosenIndex < Items.Count - 1)
        {
            chosenIndex++;
            cumulativeWeight += Items[chosenIndex].Weight;
        }
        GameObject i = Instantiate(Items[chosenIndex].gameObject, transform.position, Quaternion.identity) as GameObject;
    }

    void Update()
    {
        
    }
}
