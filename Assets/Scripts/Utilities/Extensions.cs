using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandom<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T GetRandom<T>(this List<T> array)
    {
        return array[UnityEngine.Random.Range(0, array.Count)];
    }

    public static Vector3 ToVector(this Direction direction)
    {
        if(direction == Direction.Up) return Vector2.up;
        if (direction == Direction.Down) return Vector2.down;
        if (direction == Direction.Left) return Vector2.left;
        else return Vector2.right;
    }

    public static Direction Reverse(this Direction direction)
    {
        if (direction == Direction.Up) return Direction.Down;
        if (direction == Direction.Right) return Direction.Left;
        if (direction == Direction.Left) return Direction.Right;
        else return Direction.Up;
    }

    public static bool IsHorizontal(this Direction direction)
    {
        return direction == Direction.Right || direction == Direction.Left;
    }

    public static bool IsVertical(this Direction direction)
    {
        return direction == Direction.Up || direction == Direction.Down;
    }

    public static Vector2 RightOrtho(this Vector2 vector)
    {
        return new(-vector.y, vector.x);
    }

    public static Vector2 LeftOrtho(this Vector2 vector)
    {
        return new(vector.y, -vector.x);
    }

    public static void HitEffect(this SpriteRenderer sprite, bool doFlinch = true)
    {
        sprite.material.SetFloat("_Solidity", 1);
        DOTween.To(x => 
            { 
                if (sprite != null) sprite.material.SetFloat("_Solidity", x); 
            }, 1, 0, 0.05f)
            .SetDelay(0.1f);

        //flinch
        if (!doFlinch) return;
        sprite.transform.DORewind();
        sprite.transform.DOPunchRotation(new(0, 0, UnityEngine.Random.Range(-15f, 15f)),0.2f);
        sprite.transform.DOPunchScale(new(0.2f, 0.2f, 0), 0.3f);
    }

    public static void SetGray(this SpriteRenderer sprite)
    {
        sprite.material.SetFloat("_Grayscale", 1);
    }

    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        if (degrees == 0) return vector;
        return Quaternion.Euler(0, 0, degrees) * vector;
    }

    public static Coroutine Delay(this MonoBehaviour mono, float duration, Action callback)
    {
        return mono.StartCoroutine(Utility.DelayCoroutine(duration, callback));
    }

    public static void DelayRealtime(this MonoBehaviour mono, float duration, Action callback)
    {
        mono.StartCoroutine(Utility.DelayRealtimeCoroutine(duration, callback));
    }

    public static void DelayFrame(this MonoBehaviour mono, Action callback)
    {
        mono.StartCoroutine(Utility.DelayFrameCoroutine(callback));
    }

    public static Vector2 GetDirToPlayer(this Transform transform)
    {
        return (Player.main.transform.position - transform.position).normalized;
    }

    public static Color WithAlpha(this Color col, float alpha)
    {
        col.a = alpha;
        return col;
    }
}

