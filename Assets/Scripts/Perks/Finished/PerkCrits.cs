public class PerkCrits : Perk
{
    public PerkCrits()
    {
        ID = PERK_CRITS;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    private const float ADD_AMOUNT = 0.10f;

    public override void OnFirstActive()
    {
        base.OnFirstActive();
        PlayerStats.critChance += ADD_AMOUNT * level;
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();
        PlayerStats.critChance += ADD_AMOUNT;
    }

    public override void OnDiscard()
    {
        base.OnDiscard();
        PlayerStats.critChance -= ADD_AMOUNT * level;
    }
}