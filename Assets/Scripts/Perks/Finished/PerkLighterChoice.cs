public class PerkLighterChoice : Perk
{
    public PerkLighterChoice()
    {
        ID = PERK_LIGHTERCHOICE;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.attackSpeed += 3;
        PlayerStats.attackAccuracy += 20;
        PlayerStats.damageMul -= 0.75f;
    }

    public override void OnDiscard()
    {
        PlayerStats.attackSpeed -= 3;
        PlayerStats.attackAccuracy -= 20;
        PlayerStats.damageMul += 0.75f;
    }
}
