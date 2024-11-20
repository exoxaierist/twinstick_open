using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemHeal : ShopItem
{
    public int healAmount;

    protected override void OnBuy()
    {
        Player.main.hp.Heal(new() { damage = healAmount });
    }
}
