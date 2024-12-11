using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static GameObjectPool pool = new("Coin");

    public static void Spawn(Vector2 pos, int _amount = 3)
    {
        if (LevelManager.currentRoom.coinDropAmount >= PlayerStats.roomMaxCoinCount) return; 
        Coin instance = pool.Get().GetComponent<Coin>();

        LevelManager.currentRoom.coinDropAmount += _amount;
        instance.visual = instance.GetComponent<VisualHandler>();
        instance.transform.position = pos;
        instance.amount = _amount;
        instance.ThrowCoin();
    }

    public static void Spawn(Vector2 pos, int amount, int count)
    {
        for (int i = 0; i < count; i++) Spawn(pos, amount);
    }

    public float lifetime = 10;
    public int amount = 10;
    private bool canBeCollected = false;
    private bool collected = false;
    private VisualHandler visual;
    private Coroutine releaseCoroutine;

    private void ThrowCoin()
    {
        collected = false;


        Vector3 targetPos = new();
        for (int i = 0; i < 10; i++)
        {
            targetPos = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
            if (!Physics2D.OverlapPoint(targetPos,LayerMask.GetMask("WorldStatic","PawnBlock"))) break;
        }
        transform.DOComplete();
        visual.Jump(targetPos, 0.5f, 0.4f);
        SoundSystem.Play(SoundSystem.COIN_SPAWN, transform.position,0.4f);

        canBeCollected = false;
        this.Delay(0.4f, ()=>canBeCollected = true);

        releaseCoroutine = StartCoroutine(ReleaseTimer());
    }

    private void CollectToPlayer()
    {
        if (releaseCoroutine != null) StopCoroutine(releaseCoroutine);
        collected = true;
        Vector3 from = transform.position;
        visual.sprite.gameObject.SetActive(true);
        visual.Jump(0.3f, 0.4f);
        DOTween.To(() => 0
        , x => transform.position = Vector3.Lerp(from, Player.main.transform.position, x)
        , 1f, Random.Range(0.3f, 1))
            .SetEase(Ease.InQuad)
            .SetDelay(0.31f)
            .OnComplete(() =>
            {
                SoundSystem.Play(SoundSystem.COIN_ACQUIRE, transform.position,0.4f);
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

    private IEnumerator ReleaseTimer()
    {
        yield return new WaitForSeconds(12);
        for (int i = 0; i < 7; i++)
        {
            visual.sprite.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            visual.sprite.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        canBeCollected = false;
        pool.Release(gameObject);
    }
}
