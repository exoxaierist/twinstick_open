using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class VirtualHeight : MonoBehaviour
{
    [Range(0, 1)]
    public float apexDistance = 1;
    public float apexShadowScale = 0.7f;
    private float baseHeight;
    public float height;

    [Header("Visual")]
    private Transform visual;
    private Transform shadow;
    public float shadowSize = 1;
    public Vector2 offset;

    public Action onJumpFinish;
    private bool canCallWalk = true;
    private float walkMotionTilt = 5;
    private bool isJumping = false;

    private void Awake()
    {
        CreateShadow();
        if (visual == null) visual = transform.Find("VISUAL");
        if (visual == null) return;
        baseHeight = visual.localPosition.y;
        shadow.localScale = new(0.5f, 0.5f,1);
        shadow.localPosition = offset;
    }

    public void WalkMotion()
    {
        if (!canCallWalk || isJumping) return;
        canCallWalk = false;
        walkMotionTilt *= -1;

        DOTween
            .To(SetHeight, 0, 0.25f, 0.12f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2,LoopType.Yoyo)
            .OnComplete(() => canCallWalk = true);
        visual
            .DOLocalRotate(new(0, 0, walkMotionTilt), 0.12f)
            .SetEase(Ease.OutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => visual.localRotation = Quaternion.Euler(0, 0, 0));
    }

    public void Jump(float duration) => Jump(duration, true);
    public void Jump(float duration, bool bounce)
    {
        isJumping = true;
        DOTween
            .To(SetHeight, 0, 1, duration * 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLoops(2,LoopType.Yoyo)
            .OnComplete(()=> { isJumping = false; onJumpFinish?.Invoke(); });

        if (!bounce) return;
        DOTween.To(SetHeight, 0, 0.1f, duration * 0.2f).SetEase(Ease.OutCubic).SetDelay(duration);
        DOTween.To(SetHeight, 0.1f, 0, duration * 0.2f).SetEase(Ease.InCubic).SetDelay(duration + duration*0.2f);
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
        float newScale = Mathf.Lerp(shadowSize, apexShadowScale, height) * 0.5f;
        shadow.localScale = new(newScale, newScale, 1);
        if (visual == null) return;
        visual.localPosition = new(visual.localPosition.x, Mathf.Lerp(baseHeight, baseHeight + apexDistance, height));
    }

    [ContextMenu("Create Shadow")]
    private void CreateShadow()
    {
        shadow = transform.Find("SHADOW");
        if (shadow == null)
        {
            shadow = Instantiate(Prefab.Get("Shadow")).transform;
            shadow.parent = transform;
            shadow.localPosition = offset;
        }
        shadow.localScale = new Vector3(shadowSize, shadowSize, 1) * 0.5f;
        shadow.name = "SHADOW";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere((Vector2)transform.position+offset, shadowSize * 0.5f);
    }
}
