using UnityEngine;

public class PerkAutoReload : Perk
{
    public PerkAutoReload()
    {
        ID = PERK_AUTORELOAD;
        level = 1;
        maxLevel = 2;
        OnInstantiate();
    }

    public override void OnFirstActive()
    {
        Player.onEnemyKill += Reload;
    }

    public override void OnDiscard()
    {
        Player.onEnemyKill -= Reload;
    }

    private void Reload()
    {
        if (Player.main == null || Random.Range(0,1f)>0.2f) return;
        Player.main.weapon.ammoCount = Player.main.weapon.magSize;
    }
}
