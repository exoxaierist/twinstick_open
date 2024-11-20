using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarEnemy : Enemy
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        knockBackAlpha = 0.5f;
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override void OnActivation()
    {
        StartCoroutine(ThrowBomb());
    }

    private void Update()
    {
        SetSpriteDirection(SpriteDirMode.FacePlayer);
        Movement();
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator ThrowBomb()
    {
        yield return new WaitForSeconds(Random.Range(0, 2));
        while (!hp.isDead)
        {
            if(Vector2.Distance(Player.main.transform.position,transform.position) < 9)
            {
                Bomb.Throw(transform.position, Player.main.transform.position, attackInfo);

                yield return new WaitForSeconds(Random.Range(3, 6f));
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
