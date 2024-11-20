using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class BlindBulletEnemy : Enemy
{
    private Coroutine shootRoutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 8;
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
        yield return new WaitForSeconds(Random.Range(0.5f, 5));
        while (enabled)
        {
            attackInfo.direction = transform.GetDirToPlayer().Rotate(30);
            Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
            attackInfo.direction = attackInfo.direction.Rotate(-60);
            Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);

            yield return new WaitForSeconds(Random.Range(1, 2f));
        }
    }
}
