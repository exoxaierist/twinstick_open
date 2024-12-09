public class PerkMetabolism : Perk
{
    public PerkMetabolism()
    {
        ID = PERK_METABOLISM;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.itemHealAmount += 5 * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.itemHealAmount += 5;
    }

    public override void OnDiscard()
    {
        PlayerStats.itemHealAmount -= 5 * level;
    }
}
