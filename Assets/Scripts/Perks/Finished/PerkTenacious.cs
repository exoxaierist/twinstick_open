public class PerkTenacious : Perk
{
    public PerkTenacious()
    {
        ID = PERK_TENACIOUS;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public bool isOnCooldown = false;
    public float cooldown = 5;
}
