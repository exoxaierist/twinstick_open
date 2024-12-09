public class PerkRapidFire : Perk
{
    public PerkRapidFire()
    {
        ID = PERK_RAPIDFIRE;
        level = 1;
        maxLevel = 6;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.attackSpeed += 1 * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.attackSpeed += 1;
    }

    public override void OnDiscard()
    {
        PlayerStats.attackSpeed -= 1 * level;
    }
}
