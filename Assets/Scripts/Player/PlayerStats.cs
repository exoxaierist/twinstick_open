public static class PlayerStats
{
    public static float damageMul; //base damage multiplier
    public static float attackSpeed; //attack interval = (1/n)
    public static float attackDistance; //bullet distance multiplier
    public static float attackAccuracy; //bullet accuracy (in degrees)

    public static float enemyBulletSpeedMul; //enemy bullet speed;
    public static float enemyKnockbackMul; //enemy knockback multiplier

    public static float magSize; //magsize multiplier (floor to int)
    public static float reloadDuration; //reload duration multiplier

    public static float critChance; //critical chance
    public static float critMul; //critical damage multiplier

    public static int additionalHealth; //additional player hp
    public static float moveSpeed; //move speed multiplier

    public static int roomMaxCoinCount; //max coin to get in room
    public static float coinChance; //chance of dropping coin
    public static float bananaChance; //chance of dropping banana (perk eyeforeye)
    public static int itemHealAmount; //heal item additional heal;

    public static float waveInterval; //time between each wave to start
    public static float waveChance; //chance for additional waves. goes up every room
    public static int waveEnemyCount; //enemy count per wave
    public static float enemyHpMul; //enemy hp increases every room

    public static void Reset()
    {
        damageMul = 1;
        attackSpeed = 1;
        attackDistance = 1;
        attackAccuracy = 10;

        enemyBulletSpeedMul = 1;
        enemyKnockbackMul = 1;

        magSize = 1;
        reloadDuration = 1;

        critChance = 0.05f;
        critMul = 1.5f;

        additionalHealth = 0;
        moveSpeed = 1;

        roomMaxCoinCount = 20;
        coinChance = 0.2f;
        bananaChance = 0.035f;
        itemHealAmount = 0;

        waveInterval = 10;
        waveChance = 0.1f;
        waveEnemyCount = 10;
        enemyHpMul = 1;
    }
}
