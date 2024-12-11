using System.Collections;
using UnityEngine;

public class TurretEnemy : Enemy
{
    private Coroutine shootRoutine;
    private LaserRef activeLaser;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        shootRoutine = StartCoroutine(Shoot());
        attackInfo.bulletSpeed = Bullet.enemyBulletSpeedFast;
        attackInfo.bulletMaxDist = 20;
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
                attackInfo.direction = transform.GetDirToPlayer();
                activeLaser = Laser.Spawn((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo.direction,thickness:0.1f);
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f + Vector2.up * 0.5f, attackInfo);
                knockBackAlpha = 0;
                this.Delay(1.3f, () =>
                {
                    if(!hp.isDead)
                    {
                        SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                        knockBackAlpha = 1;
                        SetMovementBehaviour(MovementBehaviour.Wander);
                    }
                });
                yield return Wait.Get(4);
            }
            else yield return Wait.Get(0.1f); 
        }
    }
}