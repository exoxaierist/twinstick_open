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
        if(distance < 8)
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        }
        else if(distance > 9)
        {
            SetMovementBehaviour(MovementBehaviour.Wander);
        }
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);

        if (hp.health <= info.damage)
        {
            if (Split()) info.damage = 0;
        }

        return info;
    }

    private bool Split()
    {
        bool spawned = SpawnImmediate(transform.position, "ENEMY_SMALLSLIME");
        if (!SpawnImmediate(transform.position, "ENEMY_SMALLSLIME") && !spawned)
        {
            return false;
        }
        else
        {
            LevelManager.currentRoom.enemyCount -= 1;
            visual.KillTweens();
            Destroy(gameObject);
            return true;
        }
    }
}
