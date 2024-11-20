public class PerkTank : Perk
{
    public PerkTank()
    {
        ID = PERK_TANK;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }
}
