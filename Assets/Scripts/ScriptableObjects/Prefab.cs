using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName ="PrefabFactory",menuName ="Prefab Factory")]
public class Prefab : ScriptableObject
{
    public static Prefab _main;
    public static Prefab main
    {
        get { 
            if(_main==null)
                _main = Resources.Load<Prefab>("PrefabLib");
            return _main; 
        }
        set { _main = value; }
    }

    public SerializedDictionary<string, GameObject> map = new();

    public static GameObject Get(string name)
    {
        if (main.map.ContainsKey(name)) return main.map[name];
        else return null;
    }
}
