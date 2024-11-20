public static class PlayerStats
{
    public static float damageMul; //base damage multiplier
    public static float attackSpeed; //attack interval = (1/n)
    public static float attackDistance; //bullet distance multiplier
    public static float attackAccuracy; //bullet accuracy (in degrees)

    public static float magSize; //magsize multiplier (floor to int)
    public static float reloadDuration; //reload duration multiplier

    public static float critChance; //critical chance
    public static float critMul; //critical damage multiplier

    public static int additionalHealth; //additional player hp
    public static float moveSpeed; //move speed multiplier

    public static float coinChance; //chance of dropping coin

    public static float waveChance; //chance for additional waves. goes up every room
    public static float enemyHpMul; //enemy hp increases every room

    public static void Reset()
    {
        damageMul = 1;
        attackSpeed = 1;
        attackDistance = 1;
        attackAccuracy = 10;

        magSize = 1;
        reloadDuration = 1;

        critChance = 0.05f;
        critMul = 1.5f;

        additionalHealth = 0;
        moveSpeed = 1;

        coinChance = 0.2f;

        waveChance = 0.1f;
        enemyHpMul = 1;
    }
}
