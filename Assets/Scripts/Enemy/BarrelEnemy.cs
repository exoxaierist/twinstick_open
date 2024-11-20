using UnityEngine;

public class BarrelEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 6;
        attackInfo.direction = Vector2.up;
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);

        if (info.attackType != AttackType.BulletHit) return info;

        attackInfo.direction = attackInfo.direction.Rotate(30);
        for (int i = 0; i < 6; i++)
        {
            Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
            attackInfo.direction = attackInfo.direction.Rotate(60);
        }

        return info;
    }
}
