using System.Collections;
using UnityEngine;

public class ClusterEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override void OnActivation()
    {
        StartCoroutine(ThrowBomb());
    }

    private void Update()
    {
        SetSpriteDirection(SpriteDirMode.FaceDirection);
        Movement();
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator ThrowBomb()
    {
        yield return Wait.Get(Random.Range(0, 2));
        while (!hp.isDead)
        {
            int count = Random.Range(2, 4);
            Vector2[] target = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                target[i] = FindEmptySpace();
            }
            foreach(Vector2 pos in target)
            {
                Bomb.Throw(transform.position, (Vector2)transform.position + pos, attackInfo);
            }

            yield return Wait.Get(Random.Range(5, 6f));
        }
    }

    private Vector2 FindEmptySpace()
    {
        Vector2 random = Vector2.zero;
        for (int i = 0; i < 100; i++)
        {
            random = Random.insideUnitCircle * 8;
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position + random, 0.5f, LayerMask.GetMask("WorldStatic", "EnemyBlock", "PawnBlock"));
            if (hit == null) break;
            random = Vector2.zero;
        }
        return random;
    }
}
