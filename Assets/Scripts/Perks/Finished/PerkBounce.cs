public class PerkBounce : Perk
{
    public PerkBounce()
    {
        ID = PERK_BOUNCE;
        level = 1;
        maxLevel = 2;
        OnInstantiate();
    }
}
