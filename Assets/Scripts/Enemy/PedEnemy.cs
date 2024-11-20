using UnityEngine;

public class PedEnemy : Enemy
{
    public Sprite greenSprite;
    public Sprite redSprite;

    private bool isRed;

    private void Update()
    {
        isRed = Vector2.Dot(transform.GetDirToPlayer(), Player.main.playerAimDirection) < -0.6f;

        if (isRed)
        {
            SetMovementBehaviour(MovementBehaviour.None);
            visual.sprite.sprite = redSprite;
        }
        else
        {
            SetMovementBehaviour(MovementBehaviour.FollowPlayer);
            visual.sprite.sprite = greenSprite;
        }

        Movement();

        //set sprite dir
        if (isRed) return;
        SetSpriteDirection(SpriteDirMode.FacePlayer);
    }
}
