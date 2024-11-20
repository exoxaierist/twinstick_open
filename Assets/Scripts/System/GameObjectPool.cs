using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private List<GameObject> list;
    private string prefabName;

    public GameObjectPool(string _prefabName)
    {
        list = new();
        prefabName = _prefabName;
    }

    public virtual GameObject Get()
    {
        if (list.Count == 0) Create();
        int index = list.Count - 1;
        GameObject item = list[index];
        if (item == null)
        {
            RemoveNull();
            Create();
            index = list.Count - 1;
            item = list[index];
        }

        list.RemoveAt(index);
        item.SetActive(true);
        return item;
    }

    public virtual void Release(GameObject item)
    {
        if (item == null) return;
        list.Add(item);
        item.transform.SetParent(null);
        item.SetActive(false);
    }

    protected virtual void Create()
    {
        Release(Object.Instantiate(Prefab.Get(prefabName)));
    }

    private void RemoveNull()
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) { list.RemoveAt(i); i--; }
        }
    }
}
