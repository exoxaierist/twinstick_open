using System.Collections;
using UnityEngine;

public class PerkRegen : Perk
{
    public PerkRegen()
    {
        ID = PERK_REGEN;
        level = 1;
        maxLevel = 5;
        OnInstantiate();
    }

    private Coroutine regenCoroutine;

    public override void OnFirstActive()
    {
    }

    private IEnumerator Regen()
    {
        while (true)
        {
            if(!LevelManager.currentRoom.isCleared && !LevelManager.currentRoom.isFriendly && Player.main!=null)
            {
                Player.main.hp.Heal(new() { damage = 5, isHeal = true });
            }
            yield return new WaitForSeconds(5-level*0.5f);
        }
    }
}
