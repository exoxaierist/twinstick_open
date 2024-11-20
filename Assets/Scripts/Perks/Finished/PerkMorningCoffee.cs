public class PerkMorningCoffee : Perk
{
    public PerkMorningCoffee()
    {
        ID = PERK_MORNINGCOFFEE;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        PlayerStats.moveSpeed += 0.1f*level;
    }

    public override void OnLevelUp()
    {
        PlayerStats.moveSpeed += 0.1f;
    }

    public override void OnDiscard()
    {
        PlayerStats.moveSpeed -= 0.1f*level;
    }
}
