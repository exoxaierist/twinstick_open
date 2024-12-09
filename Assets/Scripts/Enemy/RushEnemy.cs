using System.Collections;
using UnityEngine;

public class RushEnemy : Enemy
{
    private float slowMovespeed = 1.5f;
    private float fastMovespeed = 10;
    private Coroutine waitCoroutine;

    private Vector2 savedDirection;
      
    protected override void OnActivation()
    {
        pawn.moveSpeed = slowMovespeed;
        pawn.accelRate = 60;
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnIntervalUpdate()
    {
        CalcLineOfSight();
        if (Vector2.Distance(transform.position, Player.main.transform.position) < 10)
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
            if (hasLineOfSight && Vector2.Distance(transform.position, Player.main.transform.position) < 5)
            {
                waitCoroutine ??= StartCoroutine(WaitRoutine());
            }
        }
        else SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator WaitRoutine()
    {
        SetMovementBehaviour(MovementBehaviour.None);
        yield return new WaitForSeconds(0.7f);
        CalcLineOfSight();
        if (!hasLineOfSight)
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
            waitCoroutine = null;
            yield break;
        }
        savedDirection = transform.GetDirToPlayer();
        yield return new WaitForSeconds(0.2f);
        pawn.moveSpeed = fastMovespeed;
        float time = 0.5f;
        while(time>0)
        {
            pawn.MoveInput(savedDirection);
            time -= Time.deltaTime;
            yield return null;
        }
        pawn.moveSpeed = slowMovespeed;
        yield return new WaitForSeconds(Random.Range(0.5f,3));
        SetMovementBehaviour(MovementBehaviour.Wander);
        waitCoroutine = null;
    }
}
