using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyList",menuName ="Enemy List")]
public class EnemyList : ScriptableObject
{
    public static EnemyList _main;
    public static EnemyList main
    {
        get
        {
            if (_main == null)
                _main = Resources.Load<EnemyList>("EnemyList");
            return _main;
        }
        set { _main = value; }
    }

    public SerializedDictionary<string, GameObject> map = new();

    public static GameObject Get(string enemyID)
    {
        if (!main.map.ContainsKey(enemyID)) return null;
        return main.map[enemyID];
    }
}
