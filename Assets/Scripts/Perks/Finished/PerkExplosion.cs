public class PerkExplosion : Perk
{
    public PerkExplosion()
    {
        ID = PERK_EXPLOSION;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    public float explosionChance = 0.1f;
}
