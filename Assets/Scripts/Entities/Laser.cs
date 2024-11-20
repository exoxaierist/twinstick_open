using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public static GameObjectPool pool = new("Laser");
    public static LaserRef Spawn(Vector2 startPos, Vector2 direction, float duration = 2,float thickness = 0.1f, Action onFinish = null)
    {
        direction.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 20, LayerMask.GetMask("WorldStatic"));
        Vector2 endPos = Vector2.zero;
        if (hit) endPos = hit.point;
        else endPos = startPos + direction * 20;

        Laser instance = pool.Get().GetComponent<Laser>();
        instance.line.positionCount = 4;
        instance.line.SetPositions(new Vector3[]{
            startPos,
            startPos + (endPos-startPos).normalized * 0.3f,
            endPos + (startPos-endPos).normalized * 0.3f,
            endPos
        });
        instance.line.startWidth = thickness;
        instance.line.endWidth = thickness;
        instance.StartFlicker(duration, onFinish);

        return instance.reference;
    }

    public LaserRef reference = new();
    public LineRenderer line;

    public void StartFlicker(float duration, Action onFinish)
    {
        reference.Set(this);
        Sequence flickerSequence = DOTween.Sequence();
        LineOn();
        flickerSequence.AppendInterval(0.5f);
        for (int i = 0; i < 4; i++)
        {
            flickerSequence.AppendCallback(LineOff);
            flickerSequence.AppendInterval(0.1f);
            flickerSequence.AppendCallback(LineOn);
            flickerSequence.AppendInterval(0.1f);
        }
        flickerSequence.AppendCallback(LineOff);

        flickerSequence.Play().OnComplete(() =>
        {
            onFinish?.Invoke();
            Despawn();
        });
    }

    public void Despawn()
    {
        this.DOKill();
        LineOff();
        reference.Clear();
        pool.Release(gameObject);
    }

    private void LineOff()
    {
        line.enabled = false;
    }
    private void LineOn()
    {
        line.enabled = true;
    }
}

public class LaserRef
{
    private Laser laser;
    public Laser Get() => laser;
    public void Clear() => laser = null;
    public void Set(Laser _laser) => laser = _laser;
}
