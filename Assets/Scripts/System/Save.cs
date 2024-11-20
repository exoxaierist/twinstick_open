using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Save
{
    public static string filename = "Saves";
    public static Dictionary<string, string> dict = new();

    public static bool HasKey(string key)
    {
        LoadFile();
        return dict.ContainsKey(key);
    }

    public static void SaveFloat(string key, float value)
    {
        LoadFile();
        if (dict.ContainsKey(key)) dict[key] = SerializeFloat(value);
        else dict.Add(key, SerializeFloat(value));
        SaveFile();
    }
    public static float GetFloat(string key)
    {
        LoadFile();
        return ParseFloat(dict[key]);
    }
    public static void SaveInt(string key, int value)
    {
        LoadFile();
        if (dict.ContainsKey(key)) dict[key] = SerializeInt(value);
        else dict.Add(key, SerializeInt(value));
        SaveFile();
    }
    public static int GetInt(string key)
    {
        LoadFile();
        return ParseInt(dict[key]);
    }
    public static void SaveString(string key, string value)
    {
        LoadFile();
        if (dict.ContainsKey(key)) dict[key] = SerializeString(value);
        else dict.Add(key, SerializeString(value));
        SaveFile();
    }
    public static string GetString(string key)
    {
        LoadFile();
        return ParseString(dict[key]);
    }

    private static void SaveFile()
    {
        StringBuilder builder = new();

        foreach (KeyValuePair<string,string> pair in dict)
        {
            builder.Append(pair.Key);
            builder.Append("<~>");
            builder.Append(pair.Value);
            builder.Append("</>");
        }

        File.WriteAllText(Utility.Path(filename), builder.ToString());
    }
    private static void LoadFile()
    {
        if (!File.Exists(Utility.Path(filename))) File.WriteAllText(Utility.Path(filename), "");
        string data = File.ReadAllText(Utility.Path(filename));
        string[] pairs = data.Split("</>");

        dict.Clear();
        for (int i = 0; i < pairs.Length; i++)
        {
            string[] pair = pairs[i].Split("<~>");
            if(pair[0].Length>0) dict.Add(pair[0], pair[1]);
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Clear Save")]
    private static void ClearFile()
    {
        if (File.Exists(Utility.Path(filename)))
        {
            File.Delete(Utility.Path(filename));
            Debug.Log("File deleted: " + Utility.Path(filename));
        }
    }
#endif

    //serializers
    private static string SerializeFloat(float value)
    {
        return 'f' + value.ToString();
    }
    private static float ParseFloat(string value)
    {
        value = value.Remove(0, 1);
        return float.Parse(value);
    }
    
    private static string SerializeInt(int value)
    {
        return 'i' + value.ToString();
    }
    private static int ParseInt(string value)
    {
        value = value.Remove(0, 1);
        return int.Parse(value);
    }

    private static string SerializeString(string value)
    {
        return 's' + value;
    }
    private static string ParseString(string value)
    {
        value = value.Remove(0, 1);
        return value;
    }
}
