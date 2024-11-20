using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkDiscardElement : PickerElement
{
    public Perk perk;

    public override void OnSpawn()
    {
        info = new()
        {
            name = perk.name,
            description = perk.description
        };
    }

    public override bool CanConfirm() => true;

    public override void OnConfirm()
    {
        Player.RemovePerk(perk);
        Item.SpawnPerk(perk, Player.main.transform.position, UnityEngine.Random.insideUnitCircle); 
    }
}
