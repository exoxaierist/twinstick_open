using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMotherEnemy : Enemy
{
    private Coroutine spawnCoroutine;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetMovementBehaviour(MovementBehaviour.Wander);
        spawnCoroutine = StartCoroutine(SpawnBats());
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopCoroutine(spawnCoroutine);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    private IEnumerator SpawnBats()
    {
        yield return new WaitForSeconds(2);
        while (enabled)
        {
            SetMovementBehaviour(MovementBehaviour.None);
            for (int i = 0; i < Random.Range(1,3); i++)
            {
               this.Delay(Random.Range(0,0.5f),
                   ()=>Spawn((Vector2)transform.position + Random.insideUnitCircle*1.5f, ENEMY_BAT));
            }
            yield return new WaitForSeconds(1);
            SetMovementBehaviour(MovementBehaviour.Wander);

            yield return new WaitForSeconds(Random.Range(3, 7));
        }
    }
}
