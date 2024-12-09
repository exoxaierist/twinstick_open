using UnityEngine;

public class SmallFireSlime : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        pawn.AddForce(Random.insideUnitCircle * 10);
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        contactAttackToPlayer.attackEffect = AttackEffect.Fire;
    }

    private void Update()
    {
        Movement();
    }
}
