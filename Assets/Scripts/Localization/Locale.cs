using System;
using System.Collections.Generic;
using UnityEngine;

public static class Locale
{
    public static Language lang = Language.English;
    private static int langIndex = 0;
    public static Action onLocaleChange;

    private static List<Dictionary<string, string>> dictList = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Reload()
    {
        Parse();
    }

    public static void SetLanguage(Language newLanguage)
    {
        Parse();
        lang = newLanguage;
        langIndex = (int)lang;
        onLocaleChange?.Invoke();
    }

    public static bool ContainsKey(string key)
    {
        if (langIndex < dictList.Count) return dictList[langIndex].ContainsKey(key);
        return false;
    }

    public static string Get(string key)
    {
        if (!dictList[langIndex].ContainsKey(key)) { Debug.LogError("localization key not found: " + key); return ""; }
        return dictList[langIndex][key];
    }

    private static void Parse()
    {
        Debug.Log("Localization file loaded");
        dictList ??= new();
        dictList.Clear(); 
        TextAsset raw = Resources.Load<TextAsset>("Localization");
        string[] lines = raw.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines[0].Split(',').Length - 1; i++) dictList.Add(new());
        for (int i = 1; i < lines.Length; i++)
        {
            string[] entries = lines[i].Split(',');
            if (entries.Length<=1 || entries[0].StartsWith('#')) continue;
            if (dictList[0].ContainsKey(entries[0])) continue; //duplicate key
            for (int x = 1; x < Mathf.Min(3,entries.Length); x++)
            {
                dictList[x-1].Add(entries[0], entries[x]
                    .Replace("<heart>","<sprite name=\"heart\">")
                    .Replace("<coin>","<sprite name=\"coin\">")
                    .Replace("<shield>", "<sprite name=\"shield\">")
                    .Replace("<bnorth>", "<sprite name=\"buttonNorth\">"));
            }
        }
    }
}

public enum Language
{
    English,
    Korean,
}
