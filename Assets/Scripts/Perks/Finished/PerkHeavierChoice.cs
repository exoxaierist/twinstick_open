public class PerkHeavierChoice : Perk
{
    public PerkHeavierChoice()
    {
        ID = PERK_HEAVIERCHOICE;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.attackSpeed -= 0.4f;
        PlayerStats.damageMul += 0.6f;
    }

    public override void OnDiscard()
    {
        PlayerStats.attackSpeed += 0.4f;
        PlayerStats.damageMul -= 0.6f;
    }
}
