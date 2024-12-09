using UnityEngine;

public class GhostEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        pawn.mask = LayerMask.GetMask("");
    }

    private void Update()
    {
        pawn.MoveInput(Player.main.transform.position - transform.position);
        SetSpriteDirection(SpriteDirMode.FacePlayer);
    }
}
