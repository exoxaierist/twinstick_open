using UnityEngine;

public class SlimeEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
    }

    protected override void OnIntervalUpdate()
    {
        float distance = Vector2.Distance(Player.main.transform.position, transform.position);
        if(distance < 5)
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        }
        else if(distance > 6)
        {
            SetMovementBehaviour(MovementBehaviour.Wander);
        }
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);

        if(hp.health <= info.damage)
        {
            info.damage = 0;
            Split();
        }

        return info;
    }

    private void Split()
    {
        SpawnImmediate(transform.position, "ENEMY_SMALLSLIME");
        SpawnImmediate(transform.position, "ENEMY_SMALLSLIME");
        LevelManager.currentRoom.enemyCount -=1 ;
        visual.KillTweens();
        Destroy(gameObject);
    }
}
