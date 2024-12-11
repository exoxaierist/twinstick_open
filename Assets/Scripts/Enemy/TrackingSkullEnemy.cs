using System.Collections;
using UnityEngine;

public class TrackingSkullEnemy : Enemy
{
    private Coroutine shootRoutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletSpeed = 3 * PlayerStats.enemyBulletSpeedMul;
        attackInfo.bulletMaxDist = 8;
        attackInfo.isHoming = true;
        attackInfo.homingStrength = 2;
        attackInfo.bulletType = BulletType.Tracking;
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    protected override void OnIntervalUpdate()
    {
        CalcLineOfSight();
        if (hasLineOfSight)
        {
            SetMovementBehaviour(MovementBehaviour.Wander);
            shootRoutine ??= StartCoroutine(Shoot());
        }
        else if (Vector2.Distance(Player.main.transform.position, transform.position) < 10)
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
            if (shootRoutine != null) { StopCoroutine(shootRoutine); shootRoutine = null; }
        }
        else
        {
            SetMovementBehaviour(MovementBehaviour.Wander);
            if (shootRoutine != null) { StopCoroutine(shootRoutine); shootRoutine = null; }
        }
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    private IEnumerator Shoot()
    {
        yield return Wait.Get(Random.Range(1, 5f));
        while (enabled)
        {
            for (int i = 0; i < 2; i++)
            {
                attackInfo.direction = transform.GetDirToPlayer();
                SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                yield return Wait.Get(3.5f);
            }
            yield return Wait.Get(Random.Range(3, 5f));
        }
    }
}
