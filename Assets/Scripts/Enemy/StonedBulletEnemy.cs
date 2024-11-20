using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class StonedBulletEnemy : Enemy
{
    private Coroutine shootRoutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.isStoned = true;
        attackInfo.stoneStrength = 2f;
        attackInfo.bulletMaxDist = 12;
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);

        StopAllCoroutines();
    }

    protected override void OnIntervalUpdate()
    {
        CalcLineOfSight();
    }

    private void Update()
    {
        Movement();

        if (!isActive) return;
        if (hasLineOfSight)
        {
            SetMovementBehaviour(MovementBehaviour.Wander);
            shootRoutine ??= StartCoroutine(Shoot());
        }
        else
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
            if (shootRoutine != null) { StopCoroutine(shootRoutine); shootRoutine = null; }
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 5f));
        while (enabled)
        {
            int count = Random.Range(2, 5);
            for (int i = 0; i < count; i++)
            {
                attackInfo.direction = transform.GetDirToPlayer();
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                yield return new WaitForSeconds(2);
            }
            yield return new WaitForSeconds(Random.Range(1, 4f));
        }
    }
}
