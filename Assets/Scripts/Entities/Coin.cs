using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static GameObjectPool pool = new("Coin");

    public static void Spawn(Vector2 pos, int amount, int count)
    {
        for (int i = 0; i < count; i++) Spawn(pos, amount);
    }

    public static void Spawn(Vector2 pos, int _amount = 5)
    {
        Coin instance = pool.Get().GetComponent<Coin>();
        instance.visual = instance.GetComponent<VisualHandler>();
        instance.transform.position = pos;
        instance.amount = _amount;
        instance.collected = false;
        instance.ThrowCoin();
    }

    public int amount = 10;
    private bool canBeCollected = false;
    private bool collected = false;
    private VisualHandler visual;

    private void ThrowCoin()
    {
        Vector3 targetPos = new();
        for (int i = 0; i < 10; i++)
        {
            targetPos = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
            if (!Physics2D.OverlapPoint(targetPos)) break;
        }
        transform.DOComplete();
        visual.Jump(targetPos, 0.5f, 0.4f);
        canBeCollected = false;
        this.Delay(0.4f, ()=>canBeCollected = true);
    }

    private void CollectToPlayer()
    {
        collected = true;
        Vector3 from = transform.position;
        visual.Jump(0.3f, 0.4f);
        DOTween.To(() => 0
        , x => transform.position = Vector3.Lerp(from, Player.main.transform.position, x)
        , 1f, Random.Range(0.3f, 1))
            .SetEase(Ease.InQuad)
            .SetDelay(0.31f)
            .OnComplete(() =>
            {
                Effect.Play("Pop", EffectInfo
                    .PosRotScale(transform.position, 0 , 1f)
                    .SetLayer("Overlay"));
                Player.main.AcquireCoin(amount);
                pool.Release(gameObject);
            }); ;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (collected || !canBeCollected) return;
        if (!col.CompareTag("Player")) return;
        CollectToPlayer();
    }
}
