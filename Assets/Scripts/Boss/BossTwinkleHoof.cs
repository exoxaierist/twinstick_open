using System.Collections;
using UnityEngine;

public class BossTwinkleHoof : Boss 
{
    private float slowMoveSpeed = 3;
    private float fastMovespeed = 12;

    protected override void OnActivation()
    {
        StartCoroutine(BossRoutine());
        attackInfo.bulletMaxDist = 10;
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator BossRoutine()
    {
        while (!hp.isDead)
        {
            float random = Random.Range(0, 1f);
            if (random <= 0.6f)
            {
                //rush player
                SetMovementBehaviour(MovementBehaviour.None);
                for (int j = 0; j < 3; j++)
                {
                    yield return new WaitForSeconds(0.6f);
                    float time = 1;
                    float shootTimer = 0.2f;
                    Vector2 dir = transform.GetDirToPlayer();
                    pawn.moveSpeed = fastMovespeed;
                    while (time > 0)
                    {
                        if (shootTimer <= 0)
                        {
                            float angle = 60;
                            for (int i = 0; i < 3; i++)
                            {
                                attackInfo.direction = dir.Rotate(angle);
                                Bullet.Fire(transform.position, attackInfo);
                                angle += 120;
                            }
                            shootTimer = 0.2f;
                        }
                        pawn.MoveInput(dir);
                        shootTimer -= Time.deltaTime;
                        time -= Time.deltaTime;
                        yield return null;
                    }
                }
                pawn.moveSpeed = slowMoveSpeed;
                SetMovementBehaviour(MovementBehaviour.Wander);
            }
            else
            {
                //spawn goblins
                SetMovementBehaviour(MovementBehaviour.Wander);
                for (int i = 0; i < 2; i++)
                {
                    yield return new WaitForSeconds(2.5f);
                    for (int j = 0; j < 4; j++)
                    {
                        Spawn((Vector2)transform.position + Random.insideUnitCircle, "ENEMY_RUSH");
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(1, 4f));
        }
    }
}
