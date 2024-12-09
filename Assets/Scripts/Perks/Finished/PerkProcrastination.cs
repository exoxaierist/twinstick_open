public class PerkProcrastination : Perk
{
    public PerkProcrastination()
    {
        ID = PERK_PROCRASTINATION;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.waveInterval += 2 * level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.waveInterval += 2;
    }

    public override void OnDiscard()
    {
        PlayerStats.waveInterval -= 2 * level;
    }
}
