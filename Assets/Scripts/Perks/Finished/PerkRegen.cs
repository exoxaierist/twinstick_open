using UnityEngine;

public class PerkRegen : Perk
{
    public PerkRegen()
    {
        ID = PERK_REGEN;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        Player.onEnemyKill += OnEnemyKill;
    }

    public override void OnDiscard()
    {
        Player.onEnemyKill -= OnEnemyKill;
    }

    private void OnEnemyKill()
    {
        if (Player.main == null || Random.Range(0,1f)<0.3f) return;
        Player.main.hp.Heal(new() { damage = 1, isHeal = true });
    }
}
