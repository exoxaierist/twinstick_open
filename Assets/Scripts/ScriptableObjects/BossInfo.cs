using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName ="BossInfo",menuName ="Boss Info")]
public class BossInfo : ScriptableObject {

    public static BossInfo _main;
    public static BossInfo main
    {
        get
        {
            if (_main == null)
                _main = Resources.Load<BossInfo>("BossInfo");
            return _main;
        }
        set { _main = value; }
    }

    public static GameObject GetBoss(string id)
    {
        return main.dict[id].bossPrefab;
    }

    public static Room GetRoom(string id)
    {
        return main.dict[id].room;
    }

    public SerializedDictionary<string, BossData> dict = new();

    [System.Serializable]
    public struct BossData
    {
        public GameObject bossPrefab;
        public Room room;
    }
}
