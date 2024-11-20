using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Item
{
    public int shieldCount = 1;

    protected override void OnAcquire()
    {
        for (int i = 0; i < shieldCount; i++)
        {
            Player.main.AddShield();
        }
    }
}
