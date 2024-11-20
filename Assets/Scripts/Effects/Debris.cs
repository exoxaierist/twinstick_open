using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Debris : MonoBehaviour
{
    public static GameObjectPool pool = new("Debris");
    public static void Spawn(DebriType type, Vector2 position, Vector2 direction, float strength = 1)
    {
        Debris instance = pool.Get().GetComponent<Debris>();

        if(type == DebriType.Vase) 
            instance.sprite.sprite = instance.vaseSprite[UnityEngine.Random.Range(0, instance.vaseSprite.Count - 1)];

        instance.DOKill();
        instance.transform.SetPositionAndRotation(position, Quaternion.Euler(new(0, 0, Random.Range(0, 360))));
        strength *= Random.Range(0.3f, 1.3f);
        instance.transform.DOJump(position + direction.normalized*strength, 0.5f * strength, 1, 0.25f * strength)
            .SetEase(Ease.Linear);
        instance.transform.DORotate(new(0, 0, Random.Range(0, 360)), 0.25f * strength);

        Utility.GetMono().Delay(30, () =>
        {
            if (instance != null) pool.Release(instance.gameObject);
        });
    }

    public List<Sprite> vaseSprite = new();
    public SpriteRenderer sprite;
}

public enum DebriType
{
    None,
    Vase,
}
