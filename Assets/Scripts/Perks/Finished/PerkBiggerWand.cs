public class PerkBiggerWand : Perk
{
    public PerkBiggerWand()
    {
        ID = PERK_BIGGERWAND;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.magSize += 1 + (0.5f * (level - 1));
    }

    public override void OnLevelUp()
    {
        PlayerStats.magSize += 0.5f;
    }

    public override void OnDiscard()
    {
        PlayerStats.magSize -= 1 + (0.5f * (level - 1));
    }
}
