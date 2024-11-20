using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemy : Enemy
{
    public float maxJumpDist = 3.5f;
    
    private Collider2D col;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.Delay(Random.Range(0,3),()=>StartCoroutine(Move()));
        col = GetComponent<Collider2D>();
        attackInfo.bulletMaxDist = 5;
    }

    private IEnumerator Move()
    {
        while (enabled)
        {
            Vector2 delta = FindEmptySpace();
            pawn.Jump(delta, 0.6f);

            doContactDamage = false;
            this.Delay(0.6f, () => doContactDamage = true);

            this.Delay(0.05f, () => { visual.sprite.sortingLayerName = "Overlay"; col.enabled = false; });
            this.Delay(0.55f, () => { visual.sprite.sortingLayerName = "Default"; col.enabled = true; });


            this.Delay(0.6f, () =>
            {
                attackInfo.direction = Vector2.right;
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.left;
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.up;
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.down;
                Bullet.Fire(transform.position, attackInfo);
            });

            yield return new WaitForSeconds(3);
        }
    }

    private Vector2 FindEmptySpace()
    {
        Vector2 random = Vector2.zero;
        for (int i = 0; i < 100; i++)
        {
            random = Random.insideUnitCircle * maxJumpDist;
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position + random, 0.5f, LayerMask.GetMask("WorldStatic","EnemyBlock"));
            if (hit == null) break;
            random = Vector2.zero;
        }
        return random;
    }
}
