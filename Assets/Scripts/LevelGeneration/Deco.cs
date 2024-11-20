using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deco : MonoBehaviour
{
    public static void SpawnVase()
    {
        List<Vector2> spawnPos = new();
        int count = (int)(LevelManager.currentRoom.walkablePos.Count * 0.05f);
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                Vector2 pos = LevelManager.currentRoom.walkablePos[Random.Range(0, LevelManager.currentRoom.walkablePos.Count)];
                if (spawnPos.Contains(pos)) continue;
                spawnPos.Add(pos); 
                break;
            }
        }

        foreach (Vector2 pos in spawnPos)
        {
            GameObject instance = Instantiate(Prefab.Get(Random.Range(0,1f)<0.7f?"Vase0":"Vase1"));
            instance.transform.position = pos;
        }
    }
}
