using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossBombsmith : Boss
{
    private Vector2[] walkablePos;

    protected override void OnActivation()
    {
        base.OnActivation();
        walkablePos = LevelManager.currentRoom.walkablePos.OrderBy(x => x.y).ThenBy(x=>x.x).ToArray();
        StartCoroutine(BossRoutine());
        SetMovementBehaviour(MovementBehaviour.Wander);
        attackInfo.damage = 16;
    }

    private void Update()
    {
        Movement();
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
            float random = Random.Range(0, 1f);
            if (random <= 0.5f)
            {
                //random throw
                HashSet<Vector2> posList = new();
                int count = 5;
                float interval = 1;
                for (int c = 0; c < count; c++)
                {
                    yield return new WaitForSeconds(interval);

                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            int r = Random.Range(0, walkablePos.Length);
                            if (!posList.Contains(walkablePos[r])) { posList.Add(walkablePos[r]); break; }
                        }
                    }
                    foreach (Vector2 pos in posList)
                    {
                        this.Delay(Random.Range(0, 0.3f), () => Bomb.Throw(transform.position + Vector3.up, pos, attackInfo));
                    }
                    posList.Clear();
                }
            }
            else
            {
                //throw big at player
                for (int c = 0; c < 5; c++)
                {
                    yield return new WaitForSeconds(0.8f);
                    Bomb.Throw(transform.position+Vector3.up, Player.main.transform.position, attackInfo);
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 pos = ((Vector2)Player.main.transform.position+Player.main.pawn.unscaledVelocity) + Random.insideUnitCircle * 5f;
                        if (Physics2D.OverlapPoint(pos, LayerMask.GetMask("WorldStatic", "PawnBlock"))) continue;
                        this.Delay(Random.Range(0,0.3f),()=>Bomb.Throw(transform.position + Vector3.up, pos, attackInfo));
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(1, 4.5f));
        }
    }
}
