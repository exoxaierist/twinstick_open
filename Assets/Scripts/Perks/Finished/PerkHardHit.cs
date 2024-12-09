public class PerkHardHit : Perk
{
    public PerkHardHit()
    {
        ID = PERK_HARDHIT;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    private const float ADD_AMOUNT = 0.25f;

    public override void OnFirstActive()
    {
        base.OnFirstActive();
        PlayerStats.critMul += ADD_AMOUNT * level;
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();
        PlayerStats.critMul += ADD_AMOUNT;
    }

    public override void OnDiscard()
    {
        base.OnDiscard();
        PlayerStats.critMul -= ADD_AMOUNT * level;
    }
}