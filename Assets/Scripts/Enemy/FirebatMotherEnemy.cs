using System.Collections;
using UnityEngine;

public class FirebatMotherEnemy : Enemy
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
        yield return Wait.Get(2);
        while (enabled)
        {
            SetMovementBehaviour(MovementBehaviour.None);
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Spawn((Vector2)transform.position + Random.insideUnitCircle, "ENEMY_FIREBAT");
            }
            yield return Wait.Get(1);
            SetMovementBehaviour(MovementBehaviour.Wander);

            yield return Wait.Get(Random.Range(3, 7));
        }
    }
}
