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
        attackInfo.bulletSpeed = Bullet.enemyBulletSpeedFast;
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
        while (!hp.isDead)
        {
            if (hasLineOfSight && isActive)
            {
                SetMovementBehaviour(MovementBehaviour.None);
                activeLaser = Laser.Spawn(transform.position + Vector3.up * 0.2f, transform.GetDirToPlayer(), thickness: 0.1f);
                attackInfo.direction = transform.GetDirToPlayer();
                this.Delay(1.3f, () =>
                {
                    SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                    if (!hp.isDead) Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                    SetMovementBehaviour(MovementBehaviour.Wander);
                });
                yield return Wait.Get(4);
            }
            else yield return Wait.Get(0.1f);
        }
    }
}
