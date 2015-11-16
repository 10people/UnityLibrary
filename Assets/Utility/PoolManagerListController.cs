using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManagerListController : Singleton<PoolManagerListController>
{
    public Dictionary<string, GameObject> ItemDic = new Dictionary<string, GameObject>();

    private Dictionary<string, GameObject> parentObjectDic = new Dictionary<string, GameObject>();

    private Dictionary<string, BasePoolManager> ItemsPoolDic = new Dictionary<string, BasePoolManager>();

    public void Initialize()
    {
        if (ItemDic != null && ItemDic.Count != 0)
        {
            foreach (var effectPrefab in ItemDic)
            {
                if (!parentObjectDic.ContainsKey(effectPrefab.Key) && !ItemsPoolDic.ContainsKey(effectPrefab.Key))
                {
                    GenerateItemPoolManager(effectPrefab.Key, effectPrefab.Value);
                }
            }
        }
    }

    public void Cleanup()
    {
        //Donot destroy prefab.
        //foreach (var item in ItemDic)
        //{
        //    Destroy(item.Value);
        //}
        ItemDic.Clear();

        foreach (var pool in ItemsPoolDic)
        {
            pool.Value.Cleanup();
            pool.Value.SpawnObject = null;
        }

        ItemsPoolDic.Clear();

        foreach (var item in parentObjectDic)
        {
            Destroy(item.Value);
        }
        parentObjectDic.Clear();
    }

    /// <summary>
    /// Take pool item from pool key.
    /// </summary>
    /// <param name="key">The key of pool</param>
    public GameObject TakeItem(string key)
    {
        if (!ItemsPoolDic.ContainsKey(key))
        {
            Debug.LogError("key: " + key + " not contained.");
            return null;
        }
        var pool = ItemsPoolDic[key];
        if (pool.SpawnObject == null)
        {
            pool.SpawnObject = ItemDic[key];
        }
        return pool.Take();
    }

    /// <summary>
    /// Return game object to pool of key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="go"></param>
    public void ReturnItem(string key, GameObject go)
    {
        if (!ItemsPoolDic.ContainsKey(key))
        {
            Debug.LogError("key: " + key + " not contained.");
            return;
        }
        var pool = ItemsPoolDic[key];
        pool.Return(go);
    }

    private void GenerateItemPoolManager(string key, GameObject spawnObject)
    {
        var go = new GameObject(spawnObject.name);
        var pool = go.AddComponent<BasePoolManager>();
        pool.SpawnObject = spawnObject;
        pool.Capacity = 0;
        pool.Initialize();
        go.transform.parent = transform;
        parentObjectDic.Add(key, go);

        ItemsPoolDic.Add(key, pool);
    }

    void OnDestroy()
    {
        Cleanup();
    }
}
