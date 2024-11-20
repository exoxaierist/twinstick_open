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
        if (Player.main!=null) Activate();
        else Player.onPlayerSpawn += Activate;
    }

    public override void OnLevelUp()
    {
        PlayerStats.additionalHealth += 20;
        Player.main.hp.Damage(new () { damage = 10 });
    }

    public override void OnDiscard()
    {
        PlayerStats.additionalHealth -= 20 * level;
        Player.main.hp.Heal(new() { damage = 10,isHeal = true });
    }

    private void Activate()
    {
        Player.main.hp.Damage(new() { damage = 10 });
        Player.onPlayerSpawn -= Activate;
    }
}
