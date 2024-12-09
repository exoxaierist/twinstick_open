public class PerkFatAdrenaline : Perk
{
    public PerkFatAdrenaline()
    {
        ID = PERK_FATADRENALINE;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    private bool damageBoostApplied = false;

    public override void OnFirstActive()
    {
        if (Player.main != null) Activate();
        else Player.onPlayerSpawn += Activate;
        PlayerStats.moveSpeed -= 0.2f;
        OnHpChange();
    }

    public override void OnDiscard()
    {
        PlayerStats.moveSpeed += 0.2f;
        if (damageBoostApplied) 
        {
            PlayerStats.damageMul -= 0.5f;
            damageBoostApplied = false;
        }
        Player.onPlayerSpawn -= Activate;
    }

    private void Activate()
    {
        Player.main.hp.onDamage += OnHpChange;
        Player.main.hp.onHeal += OnHpChange;
    }

    private void OnHpChange()
    {
        if (Player.main==null) return;
        if (Player.main.hp.health > 50)
        {
            if (damageBoostApplied) { PlayerStats.damageMul -= 0.5f; damageBoostApplied = false; }
        }
        else
        {
            if(!damageBoostApplied) { PlayerStats.damageMul += 0.5f; damageBoostApplied = true; }
        }
    }
}
