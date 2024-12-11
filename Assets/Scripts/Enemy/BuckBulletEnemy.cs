using System.Collections;
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
        yield return Wait.Get(Random.Range(1, 4f));
        while (enabled)
        {
            attackInfo.direction = transform.GetDirToPlayer().Rotate(-30);
            for (int i = 0; i < 4; i++)
            {
                SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                attackInfo.direction = attackInfo.direction.Rotate(20);
            }
            yield return Wait.Get(Random.Range(3,6));
        }
    }
}
