using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    public int healAmount = 10;

    protected override void OnAcquire()
    {
        Player.main.hp.Heal(new() { damage = healAmount, isHeal = true });
    }
}
