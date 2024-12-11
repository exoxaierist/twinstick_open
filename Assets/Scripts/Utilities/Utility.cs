using System;
using System.Collections;
using UnityEngine;

public static class Utility
{
    public static IEnumerator DelayCoroutine(float duration, Action callback)
    {
        yield return Wait.Get(duration);
        callback();
    }

    public static IEnumerator DelayFrameCoroutine(Action callback)
    {
        yield return null;
        callback();
    }

    public static IEnumerator DelayRealtimeCoroutine(float duration, Action callback)
    {
        yield return new WaitForSecondsRealtime(duration);
        callback();
    }

    public static T GetRandomEnum<T>()
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public static MonoBehaviour GetMono()
    {
        return GameManager.main;
    }

    public static Color GetGray(float brightness)
    {
        return new(brightness, brightness, brightness, 1);
    }

    public static string Path(string path)
    {
        return Application.persistentDataPath + '/' + path;
    }

    public static int WeightedRandom(int[] weights)
    {
        int weightSum = 0;
        for (int i = 0; i < weights.Length; i++) weightSum += weights[i];
        
        int random = UnityEngine.Random.Range(1,weightSum+1);
        for (int i = 0; i < weights.Length; i++)
        {
            if (random <= weights[i]) return i;
            random -= weights[i];
        }

        return UnityEngine.Random.Range(0, weights.Length);
    }

    public static int GetOtherMask(Entity entity)
    {
        switch (entity)
        {
            case Entity.Player:
                return LayerMask.GetMask("Enemy");
                break;
            case Entity.Enemy:
                return LayerMask.GetMask("Player");
                break;
        }
        return 0;
    }
}
