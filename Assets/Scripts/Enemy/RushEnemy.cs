using System.Collections;
using UnityEngine;

public class RushEnemy : Enemy
{
    private float slowMovespeed = 1.5f;
    private float fastMovespeed = 10;
    private Coroutine waitCoroutine;

    private Vector2 savedPosition;
      
    protected override void OnActivation()
    {
        pawn.moveSpeed = slowMovespeed;
        pawn.accelRate = 60;
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnIntervalUpdate()
    {
        CalcLineOfSight();
        if (hasLineOfSight && Vector2.Distance(transform.position,Player.main.transform.position)<5)
        {
            waitCoroutine ??= StartCoroutine(WaitRoutine());
        }
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
        savedPosition = Player.main.transform.position;
        nav.FindPath(savedPosition);
        yield return new WaitForSeconds(0.2f);
        pawn.moveSpeed = fastMovespeed;
        while(Vector2.Distance(transform.position,savedPosition) > 1)
        {
            pawn.MoveInput(nav.GetDirection());
            yield return null;
        }
        pawn.moveSpeed = slowMovespeed;
        yield return new WaitForSeconds(Random.Range(0.5f,3));
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        waitCoroutine = null;
    }
}
