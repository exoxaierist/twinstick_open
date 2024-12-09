using UnityEngine;

public class BarrelEnemy : Enemy
{
    private bool canShoot = true;
    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 4.5f;
        attackInfo.direction = Vector2.up;
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);

        if (info.attackType != AttackType.BulletHit) return info;

        if (canShoot)
        {
            canShoot = false;
            this.Delay(0.2f, () => canShoot = true);
            attackInfo.direction = attackInfo.direction.Rotate(30);
            for (int i = 0; i < 6; i++)
            {
                SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                attackInfo.direction = attackInfo.direction.Rotate(60);
            }
        }
        

        return info;
    }
}
