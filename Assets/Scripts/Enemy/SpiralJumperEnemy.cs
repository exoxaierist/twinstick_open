using System.Collections;
using UnityEngine;

public class SpiralJumperEnemy : Enemy
{
    public float maxJumpDist = 3.5f;

    private Collider2D col;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.Delay(Random.Range(0, 3), () => StartCoroutine(Move()));
        col = GetComponent<Collider2D>();
        attackInfo.doBend = true;
        attackInfo.bendStrength = 160;
        attackInfo.bulletMaxDist = 6;
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
            SoundSystem.Play(SoundSystem.ACTION_JUMP, transform.position, 0.5f);


            this.Delay(0.6f, () =>
            {
                SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                attackInfo.direction = Vector2.right.Rotate(45);
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.left.Rotate(45);
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.up.Rotate(45);
                Bullet.Fire(transform.position, attackInfo);
                attackInfo.direction = Vector2.down.Rotate(45);
                Bullet.Fire(transform.position, attackInfo);
            });

            yield return new WaitForSeconds(4);
        }
    }

    private Vector2 FindEmptySpace()
    {
        Vector2 random = Vector2.zero;
        for (int i = 0; i < 100; i++)
        {
            random = Random.insideUnitCircle * maxJumpDist;
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position + random, 0.5f, LayerMask.GetMask("WorldStatic", "EnemyBlock", "PawnBlock"));
            if (hit == null) break;
            random = Vector2.zero;
        }
        return random;
    }
}
