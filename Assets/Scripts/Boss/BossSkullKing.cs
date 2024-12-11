using System.Collections;
using UnityEngine;

public class BossSkullKing : Boss
{
    public float distFromCenter = 2;

    private string[] childIdList = new string[4]
    {
        "ENEMY_BAT",
        "ENEMY_BULLET",
        "ENEMY_STONEDBULLET",
        "ENEMY_BUCKBULLET"
    };
    private int[] childWeightList = new int[4]
    {
        10,
        4,
        2,
        2
    };

    protected override void OnActivation()
    {
        base.OnActivation();
        StartCoroutine(BossRoutine());
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    private void Update()
    {
        Movement();
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator BossRoutine()
    {
        while (!hp.isDead)
        {
            SpawnChild(childIdList[Utility.WeightedRandom(childWeightList)], Random.Range(2, 5));
            yield return Wait.Get(Random.Range(3,7f));
        }
    }

    private void SpawnChild(string id, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Spawn((Vector2)transform.position + transform.GetDirToPlayer().Rotate(UnityEngine.Random.Range(-70, 70f)) * distFromCenter, id);
        }
    }
}
