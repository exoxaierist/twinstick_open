public class PerkHeatTreated : Perk
{
    public PerkHeatTreated()
    {
        ID = PERK_HEATTREATED;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.additionalHealth += 20 * level;
        if (Player.main!=null) QueuePlayerDamage();
        else Player.onPlayerSpawn += QueuePlayerDamage;
    }

    public override void OnLevelUp()
    {
        PlayerStats.additionalHealth += 20;
        if (Player.main != null) QueuePlayerDamage();
        else Player.onPlayerSpawn += QueuePlayerDamage;
    }

    public override void OnDiscard()
    {
        PlayerStats.additionalHealth -= 20 * level;
        Player.main.hp.Heal(new() { damage = 10,isHeal = true });
    }

    private void QueuePlayerDamage()
    {
        Player.main.hp.Damage(new() { damage = 10 });
        Player.onPlayerSpawn -= QueuePlayerDamage;
    }
}
