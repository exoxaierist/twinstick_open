using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static GameObjectPool pool = new("Explosion");
    public static void Explode(Vector2 position, float radius, AttackInfo info, float damageLerp = 0.5f)
    {
        SoundSystem.Play(SoundSystem.ACTION_EXPLOSION.GetRandom(), position);
        Effect.Play("Explosion1", EffectInfo
            .PosRotScale(position, 0, radius * 1)
            .SetLayer("Overlay"));

        for (int i = 0; i < 4; i++)
        {
            Utility.GetMono().Delay(Random.Range(0.1f, 0.3f),
                () => Effect.Play("Explosion0", EffectInfo
                .PosRotScale(position + Random.insideUnitCircle * radius*0.8f, Random.Range(-180, 180), Random.Range(0.2f, 1f))
                .SetLayer("Overlay")
                ));
        }
        for (int i = 0; i < 1; i++)
        {
            Utility.GetMono().Delay(Random.Range(0.1f, 0.3f),
                () => Effect.Play("Explosion1", EffectInfo
                .PosRotScale(position + Random.insideUnitCircle * radius*0.8f, Random.Range(-180, 180), Random.Range(0.2f, 1f))
                .SetLayer("Overlay")
                ));
        }
        CamController.main.Shake(0.07f,position);

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, Utility.GetOtherMask(info.attacker));
        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.TryGetComponent(out Hp hp))
            {
                info.damage = (int)Mathf.Lerp(info.damage,info.damage * damageLerp,Vector2.Distance(hit.transform.position,position)/radius);
                info.direction = (Vector2)hit.transform.position - position;
                info.knockBack = Bullet.defaultKnockBack;
                hp.Damage(info);
            }
        }
    }
}
