public class PerkBulletTime : Perk
{
    public PerkBulletTime()
    {
        ID = PERK_BULLETTIME;
        level = 1;
        maxLevel = 2;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.enemyBulletSpeedMul -= 0.15f * level;
    }
    public override void OnLevelUp()
    {
        PlayerStats.enemyBulletSpeedMul -= 0.15f;
    }

    public override void OnDiscard()
    {
        PlayerStats.enemyBulletSpeedMul += 0.15f * level;
    }
}
