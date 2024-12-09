using System.Collections;
using UnityEngine;

public class StoneCrawlerEnemy : Enemy
{
    private Collider2D col;
    private bool wander = true;
    private float moveDist = 1;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        col = GetComponent<Collider2D>();
        knockBackAlpha = 0.2f;
    }

    protected override void OnActivation()
    {
        StartCoroutine(Move());
    }

    protected override void OnIntervalUpdate()
    {
        if (Vector2.Distance(Player.main.transform.position, transform.position) < 10) wander = false;
        else wander = true;
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private void JumpTo(Vector2 delta)
    {
        pawn.Jump(delta, 0.5f);

        doContactDamage = false;
        this.Delay(0.5f, () => doContactDamage = true);

        this.Delay(0.05f, () => { visual.sprite.sortingLayerName = "Overlay"; col.enabled = false; });
        this.Delay(0.45f, () => { visual.sprite.sortingLayerName = "Default"; col.enabled = true; });

        SoundSystem.Play(SoundSystem.ACTION_JUMP, transform.position, 0.5f);
    }

    private Vector2 FindEmptySpace()
    {
        Vector2 random = Vector2.zero;
        for (int i = 0; i < 100; i++)
        {
            random = Random.insideUnitCircle * moveDist;
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position + random, pawn.radius+0.05f, LayerMask.GetMask("WorldStatic", "EnemyBlock", "PawnBlock"));
            if (hit == null) break;
            random = Vector2.zero;
        }
        return random;
    }

    private IEnumerator Move()
    {
        while (enabled)
        {
            if (wander)
            {
                JumpTo(FindEmptySpace());
            }
            else
            {
                nav.FindPath(Player.main.transform.position);
                Vector2 dir = nav.GetDirection().normalized;
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, pawn.radius + 0.05f, dir, moveDist,LayerMask.GetMask("WorldStatic","PawnBlock"));
                if (hit)
                {
                    dir *= (hit.centroid - (Vector2)transform.position).magnitude;
                }
                else dir *= moveDist;
                JumpTo(dir);
            }
            yield return new WaitForSeconds(2);
        }
    }
}
