using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 5;
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var pool in pools)
        {
            var objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objectQueue.Enqueue(obj);
            }

            poolDictionary[pool.key] = objectQueue;
        }
    }

    public GameObject GetFromPool(string key, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"No pool found for key: {key}");
            return null;
        }

        var poolQueue = poolDictionary[key];
        GameObject obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(pools.Find(p => p.key == key).prefab);

        obj.transform.SetParent(parent);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(string key, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[key].Enqueue(obj);
    }
}
