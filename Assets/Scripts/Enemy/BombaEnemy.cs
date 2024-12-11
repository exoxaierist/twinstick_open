using System.Collections;
using UnityEngine;

public class BombaEnemy : Enemy
{
    private bool isTicking = false;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.damage = 20;
    }

    protected override void OnActivation()
    {
        if (isTicking) return;
        SetMovementBehaviour(MovementBehaviour.FollowPlayer);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FacePlayer);
    }

    protected override void OnIntervalUpdate()
    {
        if (isTicking) return;
        float distance = Vector2.Distance(Player.main.transform.position, transform.position);
        if(distance < 3)
        {
            StartCoroutine(StartTicking());
        }
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        if(!isTicking && info.attackType == AttackType.BulletHit) StartCoroutine(StartTicking());
        info = base.OnReceiveAttack(info);
        info.damage = 0;
        return info;
    }

    private IEnumerator StartTicking()
    {
        SetMovementBehaviour(MovementBehaviour.None);
        SoundSystem.Play(SoundSystem.ENEMY_CRACKLE, transform.position, 0.5f);
        GetComponent<Collider2D>().enabled = false;
        isTicking = true;
        canBeHit = false;
        for (int i = 0; i < 5; i++)
        {
            visual.sprite.HitEffect();
            yield return Wait.Get(0.6f);
        }
        Explosion.Explode(transform.position, 2, attackInfo);
        LevelManager.currentRoom.enemyCount -= 1;
        Destroy(gameObject);
    }
}
