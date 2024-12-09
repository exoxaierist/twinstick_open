using UnityEngine;

public class EvasiveEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetMovementBehaviour(MovementBehaviour.Wander);
        knockBackAlpha = 0.5f;
    }

    protected override void OnIntervalUpdate()
    {
        if (Vector2.Distance(Player.main.transform.position, transform.position) < 8) SetMovementBehaviour(MovementBehaviour.FollowPlayer);
        else SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
        if (moveBehaviour == MovementBehaviour.FollowPlayer) SetSpriteDirection(SpriteDirMode.FacePlayer);
        else SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);
        if (info.attackType == AttackType.BulletHit && hp.health > info.damage)
        {
            int hitStrength = 8;
            Vector2 dir = transform.GetDirToPlayer();
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, dir.LeftOrtho(), 3, LayerMask.GetMask("WorldStatic", "PawnBlock"));
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, dir.RightOrtho(), 3, LayerMask.GetMask("WorldStatic", "PawnBlock"));
            if (hitLeft && !hitRight) pawn.AddForce(dir.RightOrtho() * hitStrength);
            else if (hitRight && !hitLeft) pawn.AddForce(dir.LeftOrtho() * hitStrength);
            else if (hitRight && hitLeft)
            {
                if (hitRight.distance > hitLeft.distance) pawn.AddForce(dir.RightOrtho() * hitStrength);
                else pawn.AddForce(dir.LeftOrtho() * hitStrength);
            }
            else { 
                if (Random.Range(0, 1f) > 0.5f) pawn.AddForce(dir.LeftOrtho() * hitStrength); 
                else pawn.AddForce(dir.RightOrtho() * hitStrength);
            }
        }
        return info;
    }
}
