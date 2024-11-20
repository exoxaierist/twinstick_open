using DG.Tweening;
using System;
using UnityEngine;

public class EnemySpawnEffect : MonoBehaviour
{
    public static GameObjectPool effectPool = new("EnemySpawnEffect");

    public static void SpawnEnemy(Vector3 position, Action spawnAction)
    {
        EnemySpawnEffect instance = effectPool.Get().GetComponent<EnemySpawnEffect>();
        instance.transform.SetPositionAndRotation(position, Quaternion.Euler(new(0, 0, UnityEngine.Random.Range(0, 90f))));
        instance.OnStart(spawnAction);
    }

    private SpriteRenderer spriteRenderer;
    private int flickerCount = 4;

    public void OnStart(Action onFlickerFinish)
    {
        LevelManager.currentRoom.enemyCount += 1;
        spriteRenderer = GetComponent<SpriteRenderer>();

        Sequence flickerSequence = DOTween.Sequence();
        float duration = 3;
        float initialInterval = duration / flickerCount;
        float intervalReduction = initialInterval / flickerCount;

        for (int i = 0; i < flickerCount; i++)
        {
            float currentInterval = initialInterval - (intervalReduction * i);

            flickerSequence.Append(spriteRenderer.DOFade(1, currentInterval / 2));
            flickerSequence.Append(spriteRenderer.DOFade(0, currentInterval / 2));
        }
        flickerSequence.Play().OnComplete(()=>
        {
            LevelManager.currentRoom.enemyCount -= 1;
            onFlickerFinish?.Invoke();
            effectPool.Release(gameObject);
        });
    }
}