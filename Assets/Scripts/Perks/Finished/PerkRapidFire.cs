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
        PlayerStats.attackSpeed += 0.75f * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.attackSpeed += 0.75f;
    }

    public override void OnDiscard()
    {
        PlayerStats.attackSpeed -= 0.75f * level;
    }
}
