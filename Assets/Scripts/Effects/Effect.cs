using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Effect : MonoBehaviour
{
    private static GameObjectPool playerEffectPool = new("PlayerEffect");
    private static GameObjectPool enemyEffectPool = new("EnemyEffect");
    private static GameObjectPool effectPool = new("Effect");

    private static Animator PlayInternal(GameObjectPool pool, string name, EffectInfo info)
    {
        Animator animator = pool.Get().GetComponent<Animator>();
        animator.transform.SetPositionAndRotation(info.position, Quaternion.Euler(new(0, 0, info.rotation)));
        animator.transform.localScale = info.scale;

        SpriteRenderer sprite = animator.GetComponent<SpriteRenderer>();
        sprite.color = info.color;
        sprite.sortingLayerName = info.sortingLayer;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == name)
            {
                animator.Play(name);
                Utility.GetMono().Delay(clip.length, () =>
                {
                    pool.Release(animator.gameObject);
                });
                return animator;
            }
        }
        return animator;
    }

    public static Animator PlayColored(bool isPlayer, string name, EffectInfo info) 
        => PlayInternal(isPlayer?playerEffectPool:enemyEffectPool, name, info);
    public static Animator Play(string name, EffectInfo info) 
        => PlayInternal(effectPool, name, info);
}

public struct EffectInfo
{
    public Vector3 position;
    public Vector3 scale;
    public float rotation;

    public Color color;
    public string sortingLayer;

    public static EffectInfo Pos(Vector3 position)
    {
        return new()
        {
            position = position,
            scale = new(1, 1, 1),
            rotation = 0,
            color = Color.white,
            sortingLayer = "Default"
        };
    }
    public static EffectInfo PosRotScale(Vector3 position, float rotation, float scale)
    {
        return new()
        {
            position = position,
            scale = new(scale, scale, 1),
            rotation = rotation,
            color = Color.white,
            sortingLayer = "Default"
        };
    }

    public EffectInfo SetLayer(string layer)
    {
        sortingLayer = layer;
        return this;
    }

    public EffectInfo SetColor(Color col)
    {
        color = col;
        return this;
    }
}
