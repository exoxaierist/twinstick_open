public class PerkUndying : Perk
{
    public static bool used = false;
    public PerkUndying()
    {
        ID = PERK_UNDYING;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }
}
