using System.Collections;
using UnityEngine;

public class PerkDangerzoneObject : MonoBehaviour
{
    private float _radius;
    public float radius
    {
        get { return _radius; }
        set { _radius = value; SetCircle(); }
    }
    public int damage;
    private LineRenderer line;

    private void Start()
    {
        StartCoroutine(Tick());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void SetCircle()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        float angle = 0f;
        int segments = 48;
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

    private IEnumerator Tick()
    {
        while (enabled)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hits)
            {
                if(hit.TryGetComponent(out Hp hp))
                {
                    hp.Damage(new()
                    {
                        attacker = Entity.Player,
                        damage = damage,
                        attackType = AttackType.Contact
                    });
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}
