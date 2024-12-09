using System.Collections.Generic;
using System.Linq;
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

    public List<EnemyInfo> list = new();

    public static GameObject Get(string enemyID)
    {
        return main.list.FirstOrDefault(x => x.id == enemyID).prefab;
    }

    public static List<string> GetAllID(int roomNumber = 0)
    {
        return main.list.Where(x => (x.difficulty <= roomNumber && !x.isBoss)).Select(x=>x.id).ToList();
    }

    [System.Serializable]
    public struct EnemyInfo
    {
        public string id;
        public bool isBoss;
        public int difficulty;
        public GameObject prefab;
    }
}
