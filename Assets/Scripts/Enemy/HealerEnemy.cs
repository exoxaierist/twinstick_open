using DG.Tweening;
using System.Collections;
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
        attackInfo.damage = 10;
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
        SetSpriteDirection(SpriteDirMode.FaceDirection);
    }

    protected override void OnIntervalUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10, LayerMask.GetMask("Enemy"));
        GameObject closest = null;
        float closestDist = 100000;
        foreach (Collider2D collider in hits)
        {
            if (collider.gameObject ==gameObject || Vector2.Distance(collider.transform.position, transform.position) > closestDist) continue;
            closestDist = Vector2.Distance(collider.transform.position, transform.position);
            closest = collider.gameObject;
        }
        if (closest == null || closestDist<1.5f) SetMovementBehaviour(MovementBehaviour.Wander);
        else
        {
            SetMovementBehaviour(MovementBehaviour.None);
            nav.FindPath(closest.transform.position);
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
            yield return new WaitForSeconds(2);
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
