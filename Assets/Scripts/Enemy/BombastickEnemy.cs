using UnityEngine;

public class BombastickEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.damage = 20;
    }

    private void Update()
    {
        Movement();
        if (moveBehaviour == MovementBehaviour.FollowPlayer) SetSpriteDirection(SpriteDirMode.FacePlayer);
        else SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnIntervalUpdate()
    {
        if (Vector2.Distance(Player.main.transform.position, transform.position) < 8) SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        else SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);
        if(hp.health<= info.damage)
        {
            info.damage = 0;
            Explosion.Explode(transform.position, 2, attackInfo);
            for (int c = 0; c < 5; c++)
            {
                Bomb.Throw(transform.position, (Vector2)transform.position + FindEmptySpace(), attackInfo);
            }
            LevelManager.currentRoom.enemyCount -= 1;
            Destroy(gameObject);
        }
        return info;
    }

    private Vector2 FindEmptySpace()
    {
        Vector2 random = Vector2.zero;
        for (int i = 0; i < 100; i++)
        {
            random = Random.insideUnitCircle * 3;
            Collider2D hit = Physics2D.OverlapCircle((Vector2)transform.position + random, 0.5f, LayerMask.GetMask("WorldStatic", "EnemyBlock", "PawnBlock"));
            if (hit == null) break;
            random = Vector2.zero;
        }
        return random;
    }
}
