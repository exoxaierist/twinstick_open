using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName ="SpriteLib",menuName ="Sprite Library")]
public class SpriteLib : ScriptableObject
{
    private static SpriteLib _main;
    private static SpriteLib main
    {
        get
        {
            if (_main == null) _main = Resources.Load<SpriteLib>("SpriteLib");
            return _main;
        }
    }
    public static Sprite Get(string perkId)
    {
        return main.InternalGet(perkId);
    }

    public SerializedDictionary<string,Sprite> dict = new();

    private Sprite InternalGet(string perkId)
    {
        if(!dict.ContainsKey(perkId)) { Debug.LogError("no id: " + perkId); return null; }
        return dict[perkId];
    }

    [ContextMenu("Fetch Perk IDs")]
    private void FetchPerkID()
    {
        foreach (string id in Perk.all)
        {
            if (dict.ContainsKey(id)) continue;
            dict.Add(id, dict["PERK_HOMING"]);
        }
    }
}
