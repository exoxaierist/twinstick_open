using System.Collections;
using UnityEngine;

public class PerkHomeostasis : Perk
{
    public PerkHomeostasis()
    {
        ID = PERK_HOMEOSTASIS;
        level = 1;
        maxLevel = 3;
        OnInstantiate();
    }

    private int nextTargetHP;
    private Coroutine healRoutine;

    public override void OnFirstActive()
    {
        if (Player.main == null) Activate();
        else Player.onPlayerSpawn += Activate;
    }

    public override void OnDiscard()
    {
        if (Player.main != null)
        {
            Player.main.hp.onDamage -= OnPlayerDamage;
            Player.main.hp.onHeal -= OnPlayerHeal;
        }
        Utility.GetMono().StopCoroutine(healRoutine);
    }

    private void Activate()
    {
        nextTargetHP = Player.main.hp.health;
        Player.main.hp.onDamage += OnPlayerDamage;
        Player.main.hp.onHeal += OnPlayerHeal;
        Player.onPlayerSpawn -= Activate;
    }

    private void OnPlayerHeal() => nextTargetHP = Player.main.hp.health;

    private void OnPlayerDamage()
    {
        if(healRoutine!=null) Utility.GetMono().StopCoroutine(healRoutine);
        healRoutine = Utility.GetMono().StartCoroutine(Homeostasis(nextTargetHP));
    }

    private IEnumerator Homeostasis(int targetHP)
    {
        nextTargetHP = Player.main.hp.health;

        while (Player.main.hp.health < targetHP && !LevelManager.currentRoom.isCleared)
        {
            yield return new WaitForSeconds(1);
            if (Player.main.hp == null || Player.main.hp.isDead) yield break;
            Player.main.hp.Heal(new() { damage = 1 *level ,isHeal = true});
            nextTargetHP = Player.main.hp.health;
        }
    }
}
