using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class HealerEnemy : Enemy
{
    private float radius = 2.5f;
    private int segments = 48;
    private LineRenderer line;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        attackInfo.isHeal = true;
        attackInfo.damage = 5;
        SetMovementBehaviour(MovementBehaviour.Wander);
    }

    protected override void OnActivation()
    {
        line = GetComponent<LineRenderer>();
        DrawCircle();

        line.endColor = Color.green.WithAlpha(0);
        line.startColor = Color.green.WithAlpha(0);
        StartCoroutine(HealRoutine());
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
        line.DOKill();
    }

    private void Update()
    {
        if(moveBehaviour == MovementBehaviour.None) pawn.MoveInput(nav.GetDirection());
        else { Movement(); }
    }

    protected override void OnIntervalUpdate()
    {
        Hp[] hitsHp = Physics2D.OverlapCircleAll(transform.position, 10, LayerMask.GetMask("Enemy"))
            .Select(x => x.GetComponent<Hp>()).Where(x => x != null && !x.gameObject.CompareTag("Healer")).ToArray();
        if (hitsHp.Length == 0) SetMovementBehaviour(MovementBehaviour.Wander);
        else
        {
            float min = hitsHp.Min(x => x.health / x.maxHealth);
            if (min != 1)
            {
                Hp otherHp = hitsHp.FirstOrDefault(x => (x.health / x.maxHealth) == min);
                if (Vector2.Distance(transform.position, otherHp.transform.position) < 2)
                {
                    SetMovementBehaviour(MovementBehaviour.Wander);
                }
                else
                {
                    SetMovementBehaviour(MovementBehaviour.None);
                    nav.FindPath(otherHp.transform.position);
                }
            }
            else SetMovementBehaviour(MovementBehaviour.Wander);
        }
    }

    private IEnumerator HealRoutine()
    {
        while (!hp.isDead)
        {

            line.DOColor(
                new Color2(Color.green.WithAlpha(0), Color.green.WithAlpha(0))
                , new Color2(Color.green.WithAlpha(0.2f), Color.green.WithAlpha(0.2f)), 0.2f).SetEase(Ease.Linear);
            line.DOColor(
                new Color2(Color.green.WithAlpha(0.2f), Color.green.WithAlpha(0.2f))
                , new Color2(Color.green.WithAlpha(0), Color.green.WithAlpha(0)), 1).SetDelay(0.21f).SetEase(Ease.Linear);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D collider in hits)
            {
                if (collider.gameObject == gameObject) continue;
                if (collider.TryGetComponent(out Hp hp)) hp.Heal(attackInfo);
            }
            yield return Wait.Get(2);
        }
    }

    private void DrawCircle()
    {
        float angle = 0f;
        line.positionCount = segments;
        line.loop = true;
        for (int i = 0; i < segments; i++)
        {
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += 360f / segments;
        }
    }
}
