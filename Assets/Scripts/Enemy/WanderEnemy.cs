using System.Collections;
using UnityEngine;

public class WanderEnemy : Enemy
{
    private bool hitWall = false;
    private Vector2 wallNormal;
    private bool hostile = false;
    private Coroutine behaviour;

    private void Reset()
    {
        knockBackAlpha = 10;
    }

    private void Awake()
    {
        Initialize();

        pawn = GetComponent<Pawn>();
        //pawn.onHitWall += OnHitWall;
    }

    private void Start()
    {
        behaviour = StartCoroutine(IdleBehaviour());
    }

    protected override AttackInfo OnReceiveAttack(AttackInfo info)
    {
        info = base.OnReceiveAttack(info);
        hostile = true;
        return info;
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopCoroutine(behaviour);
    }

    private void OnHitWall(Vector2 normal) { hitWall = true; wallNormal = normal; } 

    private IEnumerator IdleBehaviour()
    {
        while (enabled)
        {
            if (!hostile)
            {
                yield return Wait.Get(Random.Range(0.5f, 2));
                yield return MoveForSeconds(Random.Range(0.3f, 1.3f), Random.insideUnitCircle.normalized);
            }
            else
            {
                yield return Wait.Get(Random.Range(0.2f, 1));
                yield return MoveForSeconds(Random.Range(0.5f, 1.8f), transform.GetDirToPlayer().Rotate(Random.Range(-40f,40f)));
            }
        }
    }

    private IEnumerator MoveForSeconds(float time, Vector2 direction)
    {
        hitWall = false;
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (hostile && hitWall && -Vector2.Dot(direction, wallNormal) > 0.6f) { hostile = false; time = 0; }
            else if (!hostile && hitWall) { time = 0; yield return MoveForSeconds(0.3f, -direction.Rotate(Random.Range(-20,20))); }
            else pawn.MoveInput(direction);
            yield return null;
        }
    }
}
