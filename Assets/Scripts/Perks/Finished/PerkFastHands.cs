public class PerkFastHands : Perk
{
    public PerkFastHands()
    {
        ID = PERK_FASTHANDS;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.reloadDuration -= 0.2f*level;
    }
    public override void OnLevelUp()
    {
        PlayerStats.reloadDuration -= 0.2f;
    }
    public override void OnDiscard()
    {
        PlayerStats.reloadDuration += 0.2f * level;
    }
}
