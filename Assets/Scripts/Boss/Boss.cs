using System.Collections.Generic;
using System.Linq;

public class Boss : Enemy
{
    //
    private static string[] allBoss = new string[]
    {
        "BOSS_TWINKLEHOOF",
        "BOSS_SKULLKING",
        "BOSS_BOMBSMITH",
        "BOSS_BOB",
        "BOSS_AMALGAMATION",
    };
    //test3
    //test2
    public static HashSet<string> appearedBoss = new();
    public static string GetRandomId()
    {
        string result = allBoss.Except(appearedBoss).ToArray().GetRandom();
        appearedBoss.Add(result);
        if (appearedBoss.Count == allBoss.Length) appearedBoss.Clear();
        return result;
    }

    protected override void OnSpawn()
    {
        activationDelay = 2;
        flinchOnHit = false;
        base.OnSpawn();
        BossHP.Spawn(gameObject.name,hp);
    }

    protected override void OnDeath(AttackInfo info)
    {
        for (int c = 0; c < 5; c++)
        {
            Coin.Spawn(transform.position);
        }
        base.OnDeath(info);
    }
}
