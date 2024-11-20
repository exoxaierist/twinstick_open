public class PerkHoming : Perk
{
    public PerkHoming()
    {
        ID = PERK_HOMING;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }
}