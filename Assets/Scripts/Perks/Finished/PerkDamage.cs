public class PerkDamage : Perk
{
    public PerkDamage()
    {
        ID = PERK_DAMAGE;
        level = 1;
        maxLevel = 6;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.damageMul += 0.25f * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.damageMul += 0.25f;
    }
    public override void OnDiscard()
    {
        PlayerStats.damageMul -= 0.25f * level;
    }
}
