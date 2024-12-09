public class PerkAging : Perk
{
    public PerkAging()
    {
        ID = PERK_AGING;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }
    public float maxDamage = 5;
}
