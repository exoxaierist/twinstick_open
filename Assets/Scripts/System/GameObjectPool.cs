using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectPool
{
    private Queue<GameObject> pool;
    private string prefabName;

    public GameObjectPool(string _prefabName) { pool = new(); prefabName = _prefabName; }
    public GameObject Get()
    {
        if (pool.Count == 0) CreateElement();
        GameObject element = pool.Dequeue();
        if (element == null)
        {
            RemoveNull();
            return Get();
        }
        element.SetActive(true);
        return element;
    }
    public void Release(GameObject item)
    {
        if (item == null) return;
        item.transform.SetParent(null);
        item.SetActive(false);
        pool.Enqueue(item);
    }
    private void CreateElement() => Release(Object.Instantiate(Prefab.Get(prefabName)));
    private void RemoveNull() => pool = new(pool.Where(item => item != null));
}
