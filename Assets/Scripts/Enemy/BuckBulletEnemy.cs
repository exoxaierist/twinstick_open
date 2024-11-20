using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class BuckBulletEnemy : Enemy
{
    private Coroutine shootRoutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 3;
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
        yield return new WaitForSeconds(Random.Range(1, 4f));
        while (enabled)
        {
            attackInfo.direction = transform.GetDirToPlayer().Rotate(-30);
            for (int i = 0; i < 4; i++)
            {
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                attackInfo.direction = attackInfo.direction.Rotate(20);
            }
            yield return new WaitForSeconds(Random.Range(3,6));
        }
    }
}
