using UnityEngine;

public class MovingBarrelEnemy : Enemy
{
    private bool canShoot = true;
    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.bulletMaxDist = 5;
        attackInfo.direction = Vector2.up;
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override void OnIntervalUpdate()
    {
        float dist = Vector2.Distance(Player.main.transform.position, transform.position);
        if (dist < 5) SetMovementBehaviour(MovementBehaviour.Wander);
        else if (dist > 8) SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);

        if (info.attackType != AttackType.BulletHit) return info;
        if (canShoot)
        {
            attackInfo.direction = attackInfo.direction.Rotate(45);
            canShoot = false;
            this.Delay(0.2f, () => canShoot = true);
            for (int i = 0; i < 4; i++)
            {
                SoundSystem.Play(SoundSystem.ACTION_SHOOT_ENEMY.GetRandom(), transform.position, 0.5f);
                Bullet.Fire((Vector2)transform.position + attackInfo.direction * 0.5f, attackInfo);
                attackInfo.direction = attackInfo.direction.Rotate(90);
            }
        }
        

        return info;
    }
}
