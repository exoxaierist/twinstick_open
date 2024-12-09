using System;
using System.Collections;
using UnityEngine;

public class Hp : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public bool isDead = false;

    public Func<AttackInfo,AttackInfo> onReceiveAttack;
    public Action<AttackInfo> onDeath;
    public Action onDamage;
    public Action onHeal;

    private Coroutine fireAttack;

    private void Awake()
    {
        health = maxHealth;
    }

    public void Heal(AttackInfo info)
    {
        health += info.damage;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        for (int i = 0; i < 2; i++)
        {
            this.Delay(UnityEngine.Random.Range(0, 0.2f), () =>
            Effect.Play("Heal", EffectInfo
                .PosRotScale((Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 0.3f
                    , 0, UnityEngine.Random.Range(0.5f, 1.2f))
                .SetColor(ColorLib.healColor)
                .SetLayer("Overlay")));
        }
        DamageIndicator.Show(transform.position + Vector3.up, info);
        onHeal?.Invoke();
    }

    public void Damage(AttackInfo info)
    {
        info = (AttackInfo)onReceiveAttack?.Invoke(info);

        //fire effect
        if(info.attackEffect == AttackEffect.Fire)
        {
            if(fireAttack!=null) StopCoroutine(fireAttack);
            fireAttack = StartCoroutine(FireAttack(info));
        }

        if (info.damage == 0) return;

        health = Mathf.Max(0, health - info.damage);
        if(health <= 0 && !isDead)
        {
            isDead = true;
            if (TryGetComponent(out VisualHandler visual)) visual.FireEffect(false);
            onDeath?.Invoke(info);
        }
        DamageIndicator.Show(transform.position + Vector3.up, info);
        onDamage?.Invoke();
    }

    private IEnumerator FireAttack(AttackInfo info)
    {
        if (TryGetComponent(out VisualHandler visual)) visual.FireEffect(true);
        info.damage = 2;
        info.attackEffect = AttackEffect.None;
        info.attackType = AttackType.Fire;
        info.knockBack = 0;

        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.7f);
            if (isDead || health <= info.damage) { visual.FireEffect(false); yield break; }
            Damage(info);
        }
        if (TryGetComponent(out visual)) visual.FireEffect(false);
    }
}
