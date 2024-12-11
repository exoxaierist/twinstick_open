using System.Collections;
using UnityEngine;

public class BossAmalgamation : Boss {
    
    private float distFromCenter = 1;

    protected override void OnActivation()
    {
        SetMovementBehaviour(MovementBehaviour.Wander);
        StartCoroutine(BossRoutine());
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

    private IEnumerator BossRoutine()
    {
        yield return Wait.Get(1);
        while (!hp.isDead)
        {
            float random = Random.Range(0, 1f);
            float step = 1 / 4f;
            if(random<= step)
            {
                //shoot radial
                attackInfo.bounceCount = 0;
                attackInfo.bulletType = BulletType.Normal;
                attackInfo.isHoming = false;
                attackInfo.isStoned = false;
                attackInfo.bulletMaxDist = 30;

                for (int i = 0; i < 8; i++)
                {
                    ShootRadial(0);
                    yield return Wait.Get(0.4f);
                }
            }
            else if(random<= step * 2)
            {
                //shoot big bullet
                attackInfo.bounceCount = 0;
                attackInfo.bulletType = BulletType.Large;
                attackInfo.isStoned = true;
                attackInfo.stoneStrength = 0.8f;
                attackInfo.isHoming = false;

                AttackInfo secondaryAttackInfo = attackInfo;
                secondaryAttackInfo.bounceCount = 0;
                secondaryAttackInfo.bulletType = BulletType.Normal;
                secondaryAttackInfo.isHoming = false;
                secondaryAttackInfo.isStoned = false;

                float angle = -180 / 3f;
                for (int i = 0; i < 6; i++)
                {
                    attackInfo.direction = GetShootDir(angle);
                    attackInfo.bulletMaxDist = Random.Range(5, 15f);
                    
                    Bullet.Fire(GetShootPos(angle), attackInfo, null, x =>
                    {
                        float _angle = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            secondaryAttackInfo.direction = Vector2.up.Rotate(_angle);
                            Bullet.Fire(x.position, secondaryAttackInfo);
                            _angle += 90;
                        }
                    });
                    angle += 180/6;
                }
            }
            else if (random <= step * 3)
            {
                //shoot tracking
                attackInfo.bounceCount = 0;
                attackInfo.bulletType = BulletType.Tracking;
                attackInfo.isHoming = true;
                attackInfo.isStoned = false;
                attackInfo.homingStrength = 0.8f;
                attackInfo.bulletMaxDist = 15;
                float angle = -180 / 3f;
                for (int i = 0; i < 6; i++)
                {
                    attackInfo.direction = GetShootDir(angle);
                    Bullet.Fire(GetShootPos(angle), attackInfo);
                    angle += 20;
                }
            }
            else if (random <= step * 4)
            {
                //shoot bounce
                attackInfo.bounceCount = 2;
                attackInfo.bulletType = BulletType.Large;
                attackInfo.isHoming = false;
                attackInfo.isStoned = false;
                attackInfo.bulletMaxDist = 20;
                float angle = -360 / 4f;
                for (int i = 0; i < 8; i++)
                {
                    attackInfo.direction = GetShootDir(angle);
                    Bullet.Fire(GetShootPos(angle), attackInfo);
                    angle += 20;
                }
            }
            yield return Wait.Get(Random.Range(1, 3f));
        }
    }

    private void ShootRadial(float angleOffset)
    {
        int count = 10;
        float angleIncrement = 20;
        float angle = -angleIncrement * (count / 2f) + angleOffset;
        for (int i = 0; i < count; i++)
        {
            attackInfo.direction = GetShootDir(angle);
            Bullet.Fire(GetShootPos(angle), attackInfo);
            angle += angleIncrement;
        }
    }

    private Vector2 GetShootPos(float angle) => (Vector2)transform.position + transform.GetDirToPlayer().Rotate(angle)* distFromCenter;
    private Vector2 GetShootDir(float angle) => transform.GetDirToPlayer().Rotate(angle);
}
