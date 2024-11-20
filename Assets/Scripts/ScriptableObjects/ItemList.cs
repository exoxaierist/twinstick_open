using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemList",menuName ="Item List")]
public class ItemList : ScriptableObject
{
    public static ItemList _main;
    public static ItemList main
    {
        get
        {
            if (_main == null) _main = Resources.Load<ItemList>("ItemList");
            return _main;
        }
        set { _main = value; }
    }

    public static GameObject Get(string id)
    {
        return main.InternalGet(id);
    }
    public static string GetRandomId()
    {
        return main.InternalGetRandom();
    }

    public SerializedDictionary<string, GameObject> dict = new();
    private GameObject InternalGet(string id)
    {
        if (!dict.ContainsKey(id)) return null;
        return dict[id];
    }
    private string InternalGetRandom()
    {
        return dict.Keys.ToList()[UnityEngine.Random.Range(0, dict.Count)];
    }
}
