using UnityEngine;

public class PerkDangerzone : Perk
{
    public PerkDangerzone()
    {
        ID = PERK_DANGERZONE;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    private PerkDangerzoneObject dangerZone;

    public override void OnFirstActive()
    {
        if (Player.main == null) Player.onPlayerSpawn += Activate;
        else Activate();
    }

    public override void OnLevelUp()
    {
        if (dangerZone == null) return;
        dangerZone.radius = 3 + level;
        dangerZone.damage = 3 + level;
    }

    private void Activate()
    {
        dangerZone = Object.Instantiate(Prefab.Get("PerkDangerzoneObject")).GetComponent<PerkDangerzoneObject>();
        dangerZone.transform.SetParent(Player.main.transform);
        dangerZone.transform.localPosition = Vector3.zero;
        dangerZone.radius = 3 + level;
        dangerZone.damage = 2 + level;
    }
}
