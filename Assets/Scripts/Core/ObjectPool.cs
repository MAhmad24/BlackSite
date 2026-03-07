using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic object pool. Pre-creates GameObjects and recycles them
/// instead of using Instantiate/Destroy. Eliminates garbage collection stutters.
///
/// Usage:
///   ObjectPool pool = new ObjectPool(prefab, initialSize: 20, parent: transform);
///   GameObject obj = pool.Get(position, rotation);   // "Spawn"
///   pool.Return(obj);                                 // "Destroy" (actually recycles)
/// </summary>
public class ObjectPool
{
    private GameObject prefab;
    private Transform parent;
    private Queue<GameObject> available = new Queue<GameObject>();
    private List<GameObject> allObjects = new List<GameObject>();

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// Get an object from the pool (equivalent to Instantiate).
    /// If pool is empty, creates a new one automatically.
    /// </summary>
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (available.Count > 0)
        {
            obj = available.Dequeue();
        }
        else
        {
            obj = CreateNewObject();
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = prefab.transform.localScale;
        obj.SetActive(true);

        return obj;
    }

    /// <summary>
    /// Return an object to the pool (equivalent to Destroy).
    /// </summary>
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        available.Enqueue(obj);
    }

    /// <summary>
    /// Return ALL active objects to pool (useful for scene reset).
    /// </summary>
    public void ReturnAll()
    {
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeSelf)
            {
                Return(obj);
            }
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        available.Enqueue(obj);
        allObjects.Add(obj);
        return obj;
    }
}
