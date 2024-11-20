using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class VisualHandler : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteRenderer shadow;

    private Vector2 visualOriginPos;

    [Range(0,2)] public float shadowSize = 0.8f;
    [Range(-1,1)] public float shadowOffset;
    [Range(-2, 1)] public float globalOffset = -0.2f;

    public float baseHeight = 0;
    [Range(0,2)][SerializeField] private float _height = 0;
    public float height
    {
        get { return _height; }
        set { _height = value; UpdateVisual(); }
    }

    private float walkMotionTilt = 5;
    private bool walkCallable = true;
    private bool isJumping = false;
    private bool onFire = false;
    private Coroutine fireCoroutine;
    private Tween jumpTween;
    private Tween walkTween;

    private void OnEnable()
    {
        if (sprite == null && transform.Find("VISUAL") == null)
        {
            enabled = false;
            return;
        }
        sprite = transform.Find("VISUAL").GetComponent<SpriteRenderer>();
        visualOriginPos = sprite.transform.localPosition;

        if(shadow == null)
        {
            if (transform.Find("SHADOW") == null)
            {
                shadow = Instantiate(Prefab.Get("Shadow")).GetComponent<SpriteRenderer>();
                shadow.transform.parent = transform;
                shadow.transform.localPosition = new(0,shadowOffset,0);
            }
            shadow.transform.localScale = new Vector3(shadowSize, shadowSize, 1) * 0.5f;
            shadow.gameObject.name = "SHADOW";
        }

        UpdateVisual();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying&& shadow != null) DestroyImmediate(shadow.gameObject);
    }

    private void OnDestroy()
    {
        KillTweens();
    }

    private void OnValidate() => UpdateVisual();

    private void UpdateVisual()
    {
        if (shadow == null || sprite == null) return;
        float size = Mathf.Max(shadowSize*0.6f, Mathf.LerpUnclamped(shadowSize, shadowSize * 0.8f, height)) * 0.5f;
        shadow.transform.localScale = new(size, size, 1);
        shadow.transform.localPosition = new(0, shadowOffset + globalOffset, 0);

        sprite.transform.localPosition = new(0, height+baseHeight+globalOffset, 0);
    }

    public void Jump(Vector2 endPos, Action onLand = null)
    {
        float dist = Vector2.Distance(endPos, transform.position);
        Jump(endPos, Mathf.Min(3,dist*0.5f), Mathf.Min(1.5f,dist*0.25f), onLand);
    }

    public void Jump(Vector2 endPos,float jumpHeight ,float duration, Action onLand = null)
    {
        Jump(duration, jumpHeight, onLand);
        transform.DOMove(endPos, duration)
            .SetEase(Ease.Linear);
    }

    public void Jump(float duration, float jumpHeight = 1, Action onLand = null)
    {
        isJumping = true;
        jumpTween = DOTween
            .To(x=>height=x, 0, jumpHeight, duration * 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                isJumping = false;
                onLand?.Invoke();
            });
    }

    public void WalkMotion()
    {
        if (!walkCallable || isJumping) return;
        walkCallable = false;
        walkMotionTilt *= -1;

        walkTween = DOTween
            .To(x=>height=x, 0, 0.25f, 0.12f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => walkCallable = true);
        sprite.transform
            .DOLocalRotate(new(0, 0, walkMotionTilt), 0.12f)
            .SetEase(Ease.OutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => sprite.transform.localRotation = Quaternion.Euler(0, 0, 0));
    }

    public void FireEffect(bool state)
    {
        if (state == onFire) return;
        if (state)
        {
            if (fireCoroutine == null) fireCoroutine = StartCoroutine(FireEffect());
            sprite.color = Color.red;
        }
        else
        {
            if (fireCoroutine != null) { StopCoroutine(fireCoroutine); fireCoroutine = null; }
            sprite.color = Color.white;
        }
        onFire = state;
    }

    private IEnumerator FireEffect()
    {
        while (enabled)
        {
            Effect.Play("Fire", EffectInfo
                .PosRotScale((Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 0.3f
                    ,UnityEngine.Random.Range(-60,60),UnityEngine.Random.Range(0.8f,1.3f))
                .SetColor(Color.red));
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void KillTweens()
    {
        transform.DOKill();
        sprite.DOKill();
        shadow.DOKill();
        jumpTween?.Kill();
        walkTween?.Kill();
    }
}
