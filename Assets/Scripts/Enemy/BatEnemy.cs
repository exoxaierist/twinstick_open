
public class BatEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FacePlayer);
    }
}
