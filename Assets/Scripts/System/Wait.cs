using UnityEngine;

public static class Wait
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Reset() => CreatePool();

    private const int step = 20; // 1/n seconds of WaitForSeconds
    private const int maxSeconds = 10;
    private static WaitForSeconds[] pool;

    private static void CreatePool()
    {
        pool = new WaitForSeconds[step * maxSeconds];
        for (int i = 0; i < pool.Length; i++)
            pool[i] = new WaitForSeconds((i + 1) / (float)step);
    }

    public static WaitForSeconds Get(float seconds)
    {
        if (pool == null) CreatePool();
        int index = Mathf.RoundToInt(seconds*step)-1;
        if (index < 0) return pool[0];
        if (index > pool.Length - 1) return new WaitForSeconds(seconds);
        return pool[index];
    }
}
