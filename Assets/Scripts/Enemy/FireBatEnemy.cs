public class FireBatEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        contactAttackToPlayer.attackEffect = AttackEffect.Fire;
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FacePlayer);
    }
}
