using System.Collections;
using UnityEngine;

public class GentlemanSkullEnemy : Enemy
{
    private Coroutine shootRoutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 6;
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
        yield return Wait.Get(Random.Range(0, 1f));
        while (enabled)
        {
            attackInfo.direction = transform.GetDirToPlayer().Rotate(UnityEngine.Random.Range(-30,30f));
            SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
            Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo,bulletHit =>
            {
                if(bulletHit.type == HitColliderType.Wall)
                {
                    attackInfo.direction = bulletHit.hitInfo.normal.Rotate(-30);
                    Bullet.Fire(bulletHit.hitInfo.point + bulletHit.hitInfo.normal * 0.2f, attackInfo);
                    attackInfo.direction = bulletHit.hitInfo.normal;
                    Bullet.Fire(bulletHit.hitInfo.point + bulletHit.hitInfo.normal * 0.2f, attackInfo);
                    attackInfo.direction = bulletHit.hitInfo.normal.Rotate(30);
                    Bullet.Fire(bulletHit.hitInfo.point + bulletHit.hitInfo.normal * 0.2f, attackInfo);
                }
            });
            yield return Wait.Get(Random.Range(3, 5f));
        }
    }
}
