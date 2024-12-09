using System.Collections;
using UnityEngine;

public class BossBob : Boss
{
    private bool canShoot = true;
    private float angleOffset = 0;

    protected override void OnActivation()
    {
        base.OnActivation();
        StartCoroutine(BossRoutine());
        pawn.moveSpeed = 6;
        attackInfo.bulletMaxDist = 20;
        attackInfo.bounceCount = 1;
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        if(info.attackType == AttackType.BulletHit)
        {
            Shoot();
        }
        return base.OnReceiveAttack(info);
    }

    private IEnumerator BossRoutine()
    {
        while (!hp.isDead)
        {
            for (int c = 0; c < 5; c++)
            {
                for (int i = 0; i < 5; i++)
                {
                    Spawn((Vector2)Player.main.transform.position + Player.main.pawn.unscaledVelocity + Random.insideUnitCircle * 3, "ENEMY_BOMBA");
                }
                yield return new WaitForSeconds(1.5f);
            }
            yield return new WaitForSeconds(Random.Range(5,9));
        }
    }

    private void Shoot()
    {
        if (!canShoot) return;
        canShoot = false;
        this.Delay(0.2f, () => canShoot = true);

        float angle = 0;
        for (int i = 0; i < 8; i++)
        {
            attackInfo.direction = Vector2.up.Rotate(angle + angleOffset);
            Bullet.Fire(transform.position + Vector3.up*0.3f, attackInfo);
            angle += 360 / 8;
        }
        angleOffset += 360 / 16;
    }
}
