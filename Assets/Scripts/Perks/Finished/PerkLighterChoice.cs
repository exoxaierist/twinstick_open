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
        PlayerStats.attackSpeed += 5;
        PlayerStats.attackAccuracy += 20;
    }

    public override void OnDiscard()
    {
        PlayerStats.attackSpeed -= 5;
        PlayerStats.attackAccuracy -= 20;
    }
}
