public class PerkSlug : Perk
{
    public PerkSlug()
    {
        ID = PERK_SLUG;
        level = 1;
        maxLevel = 2;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.enemyKnockbackMul += 0.5f * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.enemyKnockbackMul += 0.5f;
    }

    public override void OnDiscard()
    {
        PlayerStats.enemyKnockbackMul -= 0.5f * level;
    }
}
