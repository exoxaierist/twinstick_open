public class PerkLongReach : Perk
{
    public PerkLongReach()
    {
        ID = PERK_LONGREACH;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.attackDistance += 0.3f * level;
    }
    public override void OnLevelUp()
    {
        PlayerStats.attackDistance += 0.3f;
    }
    public override void OnDiscard()
    {
        PlayerStats.attackDistance -= 0.3f * level;
    }
}
