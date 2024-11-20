using System.Collections;
using UnityEngine;

public class BounceTurretEnemy : Enemy
{
    private Coroutine shootRoutine;
    private LaserRef activeLaser;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        shootRoutine = StartCoroutine(Shoot());
        attackInfo.bulletSpeed = Bullet.ENEMY_BULLET_SPEED_FAST;
        attackInfo.bulletMaxDist = 20;
        attackInfo.bounceCount = 1;
    }

    protected override void OnIntervalUpdate()
    {
        CalcLineOfSight();
        if (hasLineOfSight) SetMovementBehaviour(MovementBehaviour.None);
        else SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
        if (hasLineOfSight) SetSpriteDirection(SpriteDirMode.FacePlayer);
        else SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        if (activeLaser != null && activeLaser.Get() != null) activeLaser.Get().Despawn();
        StopCoroutine(shootRoutine);
    }

    private IEnumerator Shoot()
    {
        while (!isDead)
        {
            if (hasLineOfSight && isActive)
            {
                activeLaser = Laser.Spawn(transform.position + Vector3.up * 0.2f, transform.GetDirToPlayer(), thickness: 0.1f);
                attackInfo.direction = transform.GetDirToPlayer();
                this.Delay(1.3f, () =>
                {
                    if (!isDead) Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                });
                yield return new WaitForSeconds(4);
            }
            else yield return new WaitForSeconds(0.1f);
        }
    }
}
