using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName ="SoundResource",menuName ="Sound Resource")]
public class SoundResource : ScriptableObject
{
    private static SoundResource _main;
    public static SoundResource main
    {
        set { _main = value; }
        get 
        {
            if (_main == null) _main = Resources.Load<SoundResource>("SoundResource");
            return _main;
        }
    }

    public SerializedDictionary<string, AudioClip> dict = new();
    public AudioClip defaultClip;

    public static AudioClip Get(string key)
    {
        return main.dict[key];
    }

    [ContextMenu("Fetch keys")]
    private void FetchKeys()
    {
        foreach (string key in SoundSystem.all)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, defaultClip);
        }
    }
}
