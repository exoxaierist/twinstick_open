using UnityEngine;

public class SmallSlimeEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        pawn.AddForce(Random.insideUnitCircle*10);

        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
    }
}
