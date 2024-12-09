public class PerkGoldDigger : Perk
{
    public PerkGoldDigger()
    {
        ID = PERK_GOLDDIGGER;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.coinChance += 0.1f * level;
        PlayerStats.roomMaxCoinCount += 6 * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.coinChance += 0.1f;
        PlayerStats.roomMaxCoinCount += 6;
    }

    public override void OnDiscard()
    {
        PlayerStats.coinChance -= 0.1f * level;
        PlayerStats.roomMaxCoinCount -= 6 * level;
    }
}
